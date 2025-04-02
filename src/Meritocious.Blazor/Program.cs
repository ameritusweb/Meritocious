using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Meritocious.Blazor.Services.Theme;
using Microsoft.AspNetCore.Components.Authorization;
using Meritocious.Blazor.Data;
using Meritocious.Blazor.Services.Auth;
using Meritocious.Blazor.Services.Substacks;
using Blazored.LocalStorage;
using AntDesign;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddBlazoredLocalStorage(config =>
{
    config.JsonSerializerOptions.WriteIndented = true;
});

// Configure API Clients
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001");
});

// Register API Services
builder.Services.AddScoped<INotificationApiService, NotificationApiService>();

// Register Authentication Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

// Register Ant Design
builder.Services.AddAntDesign();

// Configure HTTP client
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001");
    client.DefaultRequestHeaders.Add("X-Client-Type", "Blazor");
})
.AddHttpMessageHandler<AuthHeaderHandler>();

// Register authentication services
builder.Services.AddScoped<ITokenManager, TokenManager>();
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
    sp.GetRequiredService<ApiAuthenticationStateProvider>());

// Register API Services
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IPostApiService, PostApiService>();
builder.Services.AddScoped<ISubstackApiService, SubstackApiService>();
builder.Services.AddScoped<ITagApiService, TagApiService>();
builder.Services.AddScoped<ICommentApiService, CommentApiService>();
builder.Services.AddScoped<IModerationApiService, ModerationApiService>();
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IRemixApiService, RemixApiService>();

// Register Application Services
builder.Services.AddScoped<TabService>();
builder.Services.AddScoped<DragDropService>();
builder.Services.AddScoped<IThemeService, ThemeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
