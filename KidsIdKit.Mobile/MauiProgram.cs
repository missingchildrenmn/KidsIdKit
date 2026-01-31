using CommunityToolkit.Maui;
using KidsIdKit.Core.Data;
using KidsIdKit.Mobile.Data;
using KidsIdKit.Mobile.Services;
using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using System.Diagnostics;

namespace KidsIdKit;

public static class MauiProgram
{
    // Static reference for lifecycle events to access
    internal static ISessionService? SessionService { get; private set; }

    public static MauiApp CreateMauiApp()
    {
        try
        {
            Debug.WriteLine("🔧 MauiProgram.cs: CreateMauiApp starting");

            var builder = MauiApp.CreateBuilder();
            Debug.WriteLine("🔧 MauiProgram.cs: MauiApp.CreateBuilder() called");

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    Debug.WriteLine("🔧 MauiProgram.cs: ConfigureFonts called");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureLifecycleEvents(events =>
                {
                    Debug.WriteLine("🔧 MauiProgram.cs: ConfigureLifecycleEvents called");
#if ANDROID
                    events.AddAndroid(android => android
                        .OnStop(_ => LockSession()));
                    Debug.WriteLine("🔧 MauiProgram.cs: Android lifecycle events configured");
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
            Debug.WriteLine("🔧 MauiProgram.cs: AddMauiBlazorWebView called");

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
            Debug.WriteLine("🔧 MauiProgram.cs: Debug services configured");
#endif

            // Session service must be Singleton for lifecycle events to access
            Debug.WriteLine("🔧 MauiProgram.cs: Registering services - starting");

            builder.Services.AddSingleton<ISessionService, SessionService>();
            Debug.WriteLine("✓ MauiProgram.cs: SessionService registered");

            // Encryption service can be Singleton (no scoped dependencies)
            builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
            Debug.WriteLine("✓ MauiProgram.cs: EncryptionService registered");

            //builder.Services.AddSingleton<IPinService, PinService>();
            //Debug.WriteLine("✓ MauiProgram.cs: PinService registered");

            //// Register services - using Scoped for proper lifecycle management
            //builder.Services.AddScoped<ICompressionService, SystemCompressionService>();
            //Debug.WriteLine("✓ MauiProgram.cs: SystemCompressionService registered");

            //builder.Services.AddScoped<IStorageService, FileStorageService>();
            //Debug.WriteLine("✓ MauiProgram.cs: FileStorageService registered");

            // Storage and compression services - Singleton is fine as they have no state issues
            builder.Services.AddSingleton<ICompressionService, SystemCompressionService>();
            Debug.WriteLine("✓ MauiProgram.cs: SystemCompressionService registered");
            builder.Services.AddSingleton<IStorageService, FileStorageService>();
            Debug.WriteLine("✓ MauiProgram.cs: FileStorageService registered");

            // PinService depends on IStorageService, ISessionService, IEncryptionService, IDataAccess
            // Since IDataAccess is Scoped, PinService must also be Scoped
            builder.Services.AddScoped<IPinService, PinService>();
            Debug.WriteLine("✓ MauiProgram.cs: PinService registered");

            // Register services - using Scoped for proper lifecycle management
            builder.Services.AddScoped<IEncryptionKeyProvider, EncryptionKeyProvider>();
            Debug.WriteLine("✓ MauiProgram.cs: EncryptionKeyProvider registered");

            builder.Services.AddScoped<IDataAccess, DataAccessService>();
            Debug.WriteLine("✓ MauiProgram.cs: DataAccessService registered");

            builder.Services.AddScoped<IFamilyStateService, FamilyStateService>();
            Debug.WriteLine("✓ MauiProgram.cs: FamilyStateService registered");

            builder.Services.AddScoped<IChildHtmlRenderer, ChildHtmlRenderer>();
            Debug.WriteLine("✓ MauiProgram.cs: ChildHtmlRenderer registered");

            builder.Services.AddScoped<IFileSaverService, FileSaverService>();
            Debug.WriteLine("✓ MauiProgram.cs: FileSaverService registered");

            builder.Services.AddScoped<IFileSharerService, FileSharerService>();
            Debug.WriteLine("✓ MauiProgram.cs: FileSharerService registered");

            builder.Services.AddScoped<ICameraService, CameraService>();
            Debug.WriteLine("✓ MauiProgram.cs: CameraService registered");

            Debug.WriteLine("🔧 MauiProgram.cs: Building MauiApp...");
            var app = builder.Build();
            Debug.WriteLine("✓ MauiProgram.cs: MauiApp built successfully");

            // Store reference for lifecycle events
            Debug.WriteLine("🔧 MauiProgram.cs: Getting SessionService from DI...");
            SessionService = app.Services.GetRequiredService<ISessionService>();
            Debug.WriteLine("✓ MauiProgram.cs: SessionService stored successfully");

            Debug.WriteLine("✓ MauiProgram.cs: CreateMauiApp completed successfully");
            return app;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ MauiProgram.cs: Exception in CreateMauiApp: {ex.GetType().Name}");
            Debug.WriteLine($"❌ Message: {ex.Message}");
            Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    private static void LockSession()
    {
        try
        {
            Debug.WriteLine("🔧 MauiProgram.cs: LockSession called");
            SessionService?.Lock();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ MauiProgram.cs: Exception in LockSession: {ex.Message}");
        }
    }
}
