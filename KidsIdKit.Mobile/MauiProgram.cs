using CommunityToolkit.Maui;
using KidsIdKit.Data;
using KidsIdKit.Mobile.Data;
using KidsIdKit.Mobile.Services;
using KidsIdKit.Shared.Services;
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
			// Add additional fonts
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<IFileSaverService, FileSaverService>();
        builder.Services.AddSingleton<IFileSharerService, FileSharerService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		builder.Services.AddScoped<IDataAccess, DataAccessService>();

		return builder.Build();
	}
}
