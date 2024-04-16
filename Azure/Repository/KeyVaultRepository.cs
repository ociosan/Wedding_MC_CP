using Azure.Interfaces.Repository;
using Azure.Security.KeyVault.Secrets;

namespace Azure.Repository
{
    public class KeyVaultRepository : IKeyVaultRepository
    {
        private readonly SecretClient _secretClient;

        public KeyVaultRepository(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            KeyVaultSecret keyVaultSecret = await _secretClient.GetSecretAsync(secretName);
            return keyVaultSecret.Value;
        }

    }
}
