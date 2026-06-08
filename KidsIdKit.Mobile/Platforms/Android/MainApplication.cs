using Android.App;
using Android.Runtime;

[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage, MaxSdkVersion = 32)]

// Needed for Taking photo/video
[assembly: UsesPermission(Android.Manifest.Permission.Camera)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage, MaxSdkVersion = 32)]

// Add these properties if you would like to filter out devices that do not have cameras, or set to false to make them optional
[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]

namespace KidsIdKit;

[Application(
    AllowBackup = true,
    BackupAgent = typeof(KidsIdKit.Mobile.Platforms.Android.KidsIdKitBackupAgent),
	RestoreAnyVersion = true
)]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
