using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using KidsIdKit.Shared.Data;
using KidsIdKit.Shared.Services;
using KidsIdKit.Web.Data;
using KidsIdKit.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<KidsIdKit.Shared.Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage(); 
builder.Services.AddScoped<IDataAccess, DataAccessService>();

// Register file services
builder.Services.AddScoped<IFileSaverService, FileSaverService>();
builder.Services.AddScoped<IFileSharerService, FileSharerService>();

await builder.Build().RunAsync();
