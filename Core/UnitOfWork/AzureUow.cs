using Azure.Interfaces.Repository;
using Azure.Repository;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Core.Interfaces.UnitOfWork;

namespace Core.UnitOfWork
{
    public class AzureUow : IAzureUow
    {
        private readonly SecretClient _secretClient;
        private readonly BlobServiceClient _blobServiceClient;

        private IKeyVaultRepository _keyVault;
        private IStorageAccountRepository _storageAccount;
        private IServiceBusRepository _serviceBusRepository;

        public AzureUow(SecretClient secretClient, BlobServiceClient blobServiceClient)
        {
            _secretClient = secretClient;
            _blobServiceClient = blobServiceClient;
        }

        public IKeyVaultRepository KeyVault => _keyVault ??= new KeyVaultRepository(_secretClient);
        public IStorageAccountRepository StorageAccount => _storageAccount ??= new StorageAccountRepository(_blobServiceClient);
        public IServiceBusRepository ServiceBus => _serviceBusRepository ??= new ServiceBusRepository();


    }
}
