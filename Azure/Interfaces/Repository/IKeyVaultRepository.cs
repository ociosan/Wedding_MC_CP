namespace Azure.Interfaces.Repository
{
    public interface IKeyVaultRepository
    {
        Task<string> GetSecretAsync(string secretName)
    }
}
