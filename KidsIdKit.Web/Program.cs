using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using KidsIdKit.Web.Data;
using KidsIdKit.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<KidsIdKit.Core.Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ICompressionService, SharpZipCompressionService>();
builder.Services.AddScoped<IStorageService, LocalStorageService>();
builder.Services.AddScoped<IEncryptionKeyProvider, EncryptionKeyProvider>();
builder.Services.AddScoped<IDataAccess, DataAccessService>();
builder.Services.AddScoped<IFamilyStateService, FamilyStateService>();
builder.Services.AddScoped<IChildHtmlRenderer, ChildHtmlRenderer>();

// Register file services
builder.Services.AddScoped<IFileSaverService, FileSaverService>();
builder.Services.AddScoped<IFileSharerService, FileSharerService>();

await builder.Build().RunAsync();
