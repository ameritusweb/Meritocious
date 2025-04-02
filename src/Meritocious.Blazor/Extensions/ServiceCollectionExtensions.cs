using Microsoft.Extensions.DependencyInjection;
using Meritocious.Blazor.Services.Auth;
using Meritocious.Blazor.Services.Api;
using Meritocious.Blazor.Services.Substacks;
using Meritocious.Blazor.Services.Theme;
using Blazored.LocalStorage;

namespace Meritocious.Blazor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorServices(this IServiceCollection services)
        {
            // Add core Blazor services
            services.AddRazorPages();
            services.AddServerSideBlazor();
            
            // Add local storage
            services.AddBlazoredLocalStorage(config =>
            {
                config.JsonSerializerOptions.WriteIndented = true;
            });

            // Add API clients
            services.AddHttpClient<ApiClient>();
            services.AddScoped<IAuthApiService, AuthApiService>();
            services.AddScoped<IUserApiService, UserApiService>();
            services.AddScoped<IPostApiService, PostApiService>();
            services.AddScoped<ISubstackApiService, SubstackApiService>();
            services.AddScoped<ITagApiService, TagApiService>();
            services.AddScoped<ICommentApiService, CommentApiService>();
            services.AddScoped<IModerationApiService, ModerationApiService>();
            services.AddScoped<IRemixApiService, RemixApiService>();
            services.AddScoped<INotificationApiService, NotificationApiService>();

            // Add application services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            services.AddScoped<ISubstackService, SubstackService>();
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<TabService>();
            services.AddScoped<DragDropService>();

            // Add security and auth pipeline
            services.AddScoped<AuthHeaderHandler>();
            services.AddScoped<ApiAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(sp => 
                sp.GetRequiredService<ApiAuthenticationStateProvider>());

            return services;
        }

        public static IServiceCollection AddSecurityServices(this IServiceCollection services)
        {
            services.AddScoped<ISecurityAuditService, SecurityAuditService>();
            services.AddScoped<SecurityExceptionHandler>();
            services.AddScoped<AuditLoggingMiddleware>();

            services.AddAuthorization(options =>
            {
                // Admin policies
                options.AddPolicy("ViewAuditLogs", policy =>
                    policy.RequireRole("Administrator")
                          .RequireClaim("Permissions", "ViewAuditLogs"));
                
                options.AddPolicy("ExportAuditLogs", policy =>
                    policy.RequireRole("Administrator")
                          .RequireClaim("Permissions", "ExportAuditLogs"));
                
                options.AddPolicy("ManageUsers", policy =>
                    policy.RequireRole("Administrator")
                          .RequireClaim("Permissions", "ManageUsers"));
                
                options.AddPolicy("ViewSecurityEvents", policy =>
                    policy.RequireRole("Administrator")
                          .RequireClaim("Permissions", "ViewSecurityEvents"));

                // Moderation policies
                options.AddPolicy("ModerateContent", policy =>
                    policy.RequireRole("Administrator", "Moderator")
                          .RequireClaim("Permissions", "ModerateContent"));
                
                options.AddPolicy("ViewModerationQueue", policy =>
                    policy.RequireRole("Administrator", "Moderator")
                          .RequireClaim("Permissions", "ViewModerationQueue"));
            });

            return services;
        }
    }
}