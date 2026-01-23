using CommunityToolkit.Maui;
using KidsIdKit.Core.Data;
using KidsIdKit.Mobile.Data;
using KidsIdKit.Mobile.Services;
using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace KidsIdKit;

public static class MauiProgram
{
    // Static reference for lifecycle events to access
    internal static ISessionService? SessionService { get; private set; }

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android
                    .OnStop(_ => LockSession()));
#elif IOS || MACCATALYST
                events.AddiOS(ios => ios
                    .DidEnterBackground(_ => LockSession()));
#elif WINDOWS
                events.AddWindows(windows => windows
                    .OnVisibilityChanged((_, args) =>
                    {
                        if (!args.Visible)
                            LockSession();
                    }));
#endif
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Session service must be Singleton for lifecycle events to access
        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
        builder.Services.AddSingleton<IPinService, PinService>();

        // Register services - using Scoped for proper lifecycle management
        builder.Services.AddScoped<ICompressionService, SystemCompressionService>();
        builder.Services.AddScoped<IStorageService, FileStorageService>();
        builder.Services.AddScoped<IEncryptionKeyProvider, EncryptionKeyProvider>();
        builder.Services.AddScoped<IDataAccess, DataAccessService>();
        builder.Services.AddScoped<IFamilyStateService, FamilyStateService>();
        builder.Services.AddScoped<IChildHtmlRenderer, ChildHtmlRenderer>();
        builder.Services.AddScoped<IFileSaverService, FileSaverService>();
        builder.Services.AddScoped<IFileSharerService, FileSharerService>();
        builder.Services.AddScoped<ICameraService, CameraService>();

        var app = builder.Build();

        // Store reference for lifecycle events
        SessionService = app.Services.GetRequiredService<ISessionService>();

        return app;
    }

    private static void LockSession()
    {
        SessionService?.Lock();
    }
}
