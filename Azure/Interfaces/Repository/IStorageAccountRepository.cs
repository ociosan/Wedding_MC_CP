namespace Azure.Interfaces.Repository
{
    public interface IStorageAccountRepository
    {
        Task<byte[]> DownloadInvitationTemplateAsync();
        Task UploadInvitationCodeAsync(Stream fileContent, string invitationCode, string fileType);
        Task<byte[]> DownloadInvitationAsync(string invitationCode, string fileType);
        Task<bool> FileExistsAsync(string invitationCode, string fileType);
    }
}
