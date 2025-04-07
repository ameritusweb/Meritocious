using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces;

public interface ISecretsService
{
    Task<string> GetSecretAsync(string secretName);
    Task<T> GetSecretAsync<T>(string secretName) where T : class;
    Task<bool> SecretExistsAsync(string secretName);
}