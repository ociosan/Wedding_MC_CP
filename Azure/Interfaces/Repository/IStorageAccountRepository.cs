namespace Azure.Interfaces.Repository
{
    public interface IStorageAccountRepository
    {
        Task<byte[]> DownloadInvitationTemplateAsync();
        Task UploadInvitationCodeAsync(Stream fileContent, string invitationCode);
    }
}
