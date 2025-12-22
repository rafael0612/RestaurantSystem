using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RestaurantSystem.Client;
using RestaurantSystem.Client.Auth;
using RestaurantSystem.Client.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ITokenStorage, TokenStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddTransient<BearerTokenHandler>();
builder.Services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    Converters = { new JsonStringEnumConverter() }
});

var apiBase = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5013";

// HttpClient con Bearer automático
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBase);
})
.AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

// API services
builder.Services.AddScoped<AuthApi>();
builder.Services.AddScoped<UsuariosApi>();
builder.Services.AddScoped<MeseroApi>();
builder.Services.AddScoped<CocinaApi>();
builder.Services.AddScoped<CajaApi>();
builder.Services.AddScoped<ProductosApi>();
builder.Services.AddScoped<MeseroApi>();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
