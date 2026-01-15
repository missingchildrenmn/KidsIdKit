using CommunityToolkit.Maui;
using KidsIdKit.Core.Data;
using KidsIdKit.Mobile.Data;
using KidsIdKit.Mobile.Services;
using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;

namespace KidsIdKit;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

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

        return builder.Build();
    }
}
