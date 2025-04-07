using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Meritocious.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Meritocious.Infrastructure.Services;

public class SecretsService : ISecretsService
{
    private readonly IConfiguration configuration;
    private readonly IMemoryCache cache;
    private readonly SecretClient? keyVaultClient;
    private readonly bool isDevelopment;
    private readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(30);

    public SecretsService(IConfiguration configuration, IMemoryCache cache, IWebHostEnvironment env)
    {
        this.configuration = configuration;
        this.cache = cache;
        isDevelopment = env.IsDevelopment();

        if (!isDevelopment)
        {
            var keyVaultUrl = this.configuration["KeyVault:Url"];
            keyVaultClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        var cacheKey = $"Secret_{secretName}";

        if (cache.TryGetValue(cacheKey, out string cachedValue))
        {
            return cachedValue;
        }

        string secretValue;

        if (isDevelopment)
        {
            secretValue = Environment.GetEnvironmentVariable(secretName) ?? 
                         throw new KeyNotFoundException($"Environment variable {secretName} not found");
        }
        else
        {
            var secret = await keyVaultClient!.GetSecretAsync(secretName);
            secretValue = secret.Value.Value;
        }

        cache.Set(cacheKey, secretValue, cacheDuration);
        return secretValue;
    }

    public async Task<T> GetSecretAsync<T>(string secretName) where T : class
    {
        var secretValue = await GetSecretAsync(secretName);
        return System.Text.Json.JsonSerializer.Deserialize<T>(secretValue) ?? 
               throw new InvalidOperationException($"Could not deserialize secret {secretName} to type {typeof(T).Name}");
    }

    public async Task<bool> SecretExistsAsync(string secretName)
    {
        try
        {
            if (isDevelopment)
            {
                return Environment.GetEnvironmentVariable(secretName) != null;
            }
            
            await keyVaultClient!.GetSecretAsync(secretName);
            return true;
        }
        catch
        {
            return false;
        }
    }
}