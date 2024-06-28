using Azure.Interfaces.Repository;

namespace Core.Interfaces.UnitOfWork
{
    public interface IAzureUow
    {
        public IKeyVaultRepository KeyVault { get; }
        public IStorageAccountRepository StorageAccount { get; }
        public IServiceBusRepository ServiceBus { get; }

    }
}
