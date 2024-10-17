using Blazored.LocalStorage;
using KidsIdKit.Data;
using KidsIdKit.Shared;
using KidsIdKit.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage(); 
builder.Services.AddScoped<IDataAccess, DataAccessService>();

await builder.Build().RunAsync();
