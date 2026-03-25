using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Biometric authentication service for MAUI.
/// Uses platform-specific biometric APIs (Android BiometricPrompt, iOS LAContext).
/// </summary>
public class BiometricService : IBiometricService
{

    public Task<bool> IsAvailableAsync()
    {
#if ANDROID
        try
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(29))
                return Task.FromResult(false);

            var manager = (Android.Hardware.Biometrics.BiometricManager?)
                Platform.AppContext.GetSystemService(Android.Content.Context.BiometricService);
            if (manager == null)
                return Task.FromResult(false);

            const int BIOMETRIC_SUCCESS = 0;
#pragma warning disable CA1422 // CanAuthenticate() obsoleted on API 30+ but still functional
            var result = manager.CanAuthenticate();
#pragma warning restore CA1422
            return Task.FromResult((int)result == BIOMETRIC_SUCCESS);
        }
        catch
        {
            return Task.FromResult(false);
        }
#elif IOS || MACCATALYST
        var context = new LocalAuthentication.LAContext();
        var canEvaluate = context.CanEvaluatePolicy(LocalAuthentication.LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out _);
        return Task.FromResult(canEvaluate);
#else
        return Task.FromResult(false);
#endif
    }

    public async Task<bool> AuthenticateAsync(string reason)
    {
#if ANDROID
        if (!OperatingSystem.IsAndroidVersionAtLeast(28))
            return false;

        var tcs = new TaskCompletionSource<bool>();

#pragma warning disable CA1416 // Platform compatibility - guarded by IsAndroidVersionAtLeast(28) above
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            try
            {
                var activity = Platform.CurrentActivity;
                if (activity == null)
                {
                    tcs.TrySetResult(false);
                    return;
                }

                var executor = activity.MainExecutor;
                if (executor == null)
                {
                    tcs.TrySetResult(false);
                    return;
                }

                var builder = new Android.Hardware.Biometrics.BiometricPrompt.Builder(activity);
                builder.SetTitle("Fingerprint Sign-in");
                builder.SetDescription(reason);
                builder.SetNegativeButton("Cancel", executor, new NegativeButtonClickListener(tcs));

                var prompt = builder.Build();
                prompt.Authenticate(new Android.OS.CancellationSignal(), executor, new BiometricAuthCallback(tcs));
            }
            catch
            {
                tcs.TrySetResult(false);
            }
        });
#pragma warning restore CA1416

        return await tcs.Task;
#elif IOS || MACCATALYST
        var context = new LocalAuthentication.LAContext();
        if (!context.CanEvaluatePolicy(LocalAuthentication.LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out _))
            return false;

        var (success, _) = await context.EvaluatePolicyAsync(
            LocalAuthentication.LAPolicy.DeviceOwnerAuthenticationWithBiometrics, reason);
        return success;
#else
        await Task.CompletedTask;
        return false;
#endif
    }
}

#if ANDROID
[System.Runtime.Versioning.SupportedOSPlatform("android28.0")]
internal class BiometricAuthCallback : Android.Hardware.Biometrics.BiometricPrompt.AuthenticationCallback
{
    private readonly TaskCompletionSource<bool> _tcs;

    public BiometricAuthCallback(TaskCompletionSource<bool> tcs)
    {
        _tcs = tcs;
    }

    public override void OnAuthenticationSucceeded(Android.Hardware.Biometrics.BiometricPrompt.AuthenticationResult? result)
    {
        base.OnAuthenticationSucceeded(result);
        _tcs.TrySetResult(true);
    }

    public override void OnAuthenticationError(Android.Hardware.Biometrics.BiometricErrorCode errorCode, Java.Lang.ICharSequence? errString)
    {
        base.OnAuthenticationError(errorCode, errString);
        _tcs.TrySetResult(false);
    }

    public override void OnAuthenticationFailed()
    {
        base.OnAuthenticationFailed();
        // Individual attempt failed; the prompt stays open for retry
    }
}

[System.Runtime.Versioning.SupportedOSPlatform("android28.0")]
internal class NegativeButtonClickListener : Java.Lang.Object, Android.Content.IDialogInterfaceOnClickListener
{
    private readonly TaskCompletionSource<bool> _tcs;

    public NegativeButtonClickListener(TaskCompletionSource<bool> tcs)
    {
        _tcs = tcs;
    }

    public void OnClick(Android.Content.IDialogInterface? dialog, int which)
    {
        _tcs.TrySetResult(false);
    }
}
#endif
