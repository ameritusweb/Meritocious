using Meritocious.Infrastructure;
using Meritocious.AI;
using System.Text.Json.Serialization;
using Meritocious.Core.Interfaces;
using Meritocious.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AspNetCoreRateLimit;
using Meritocious.Web.Middleware;
using Meritocious.Web.Hubs;
using Meritocious.Web.ExceptionHandling;
using Microsoft.AspNetCore.Authorization;
using Meritocious.Web.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring OpenAPI/Swagger at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(
    cfg => 
    {
        cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    });

// Add infrastructure services
builder.Services.AddMeritociousInfrastructure(builder.Configuration);

// Configure rate limiting
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Configure IP rate limiting
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configure client rate limiting
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();

builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));

builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Add AI services
builder.Services.AddMeritociousAI(builder.Configuration);

// Register auth services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new GoogleLinkedRequirement())
        .Build();

    options.AddPolicy("NoGoogleRequired", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.AddScoped<IAuthorizationHandler, GoogleLinkedHandler>();

// Add SignalR
builder.Services.AddSignalR();

// Add CORS with proper configuration for Google Sign-In and SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000", "http://localhost:5001") // Added mock API origin
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure HttpClient for mock API in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpClient("MockApi", client =>
    {
        client.BaseAddress = new Uri("http://localhost:5001/");
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add custom security middleware
app.UseMiddleware<AuditLoggingMiddleware>();

// Configure exception handling
// TODO: Configure this.
// app.UseExceptionHandler(options => {
//    options.ExceptionHandlers.Add<SecurityExceptionHandler>();
// });

// Add rate limiting middleware
app.UseIpRateLimiting();
app.UseClientRateLimiting();

// Configure security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://accounts.google.com https://apis.google.com; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "connect-src 'self' https://apis.google.com; " +
        "frame-src https://accounts.google.com;");
    await next();
});

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Initialize the database
await Meritocious.Infrastructure.Data.DbInitializer.InitializeAsync(app.Services);
await Meritocious.Infrastructure.Data.DbInitializer.SeedAdminUserAsync(app.Services);

app.Run();