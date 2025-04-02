using Microsoft.Extensions.DependencyInjection;

namespace Meritocious.Blazor.Services.Api;

public static class ApiClientFactory
{
    public static void AddApiClient(this IServiceCollection services, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.AddHttpClient<ApiClient>("MockApi", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5001/");
            });
        }
        else
        {
            services.AddHttpClient<ApiClient>();
        }
    }
}