using Azure.Interfaces.Repository;
using Azure.Storage.Blobs;

namespace Azure.Repository
{
    public class StorageAccountRepository : IStorageAccountRepository
    {
        private readonly BlobServiceClient _blobServiceClient;

        public StorageAccountRepository(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<byte[]> DownloadInvitationTemplateAsync()
        {
            byte[] theInvitationBytes;

            await using (MemoryStream memoryStream = new())
            {
                var client = _blobServiceClient.GetBlobContainerClient("invitations").GetBlobClient("FamilyConfirmation.pdf");
                await client.DownloadToAsync(memoryStream);
                theInvitationBytes = memoryStream.ToArray();
            }

            return theInvitationBytes;
        }

        public async Task UploadInvitationCodeAsync(Stream fileContent, string invitationCode, string fileType)
        {
            await using Stream str = fileContent;
            var client = _blobServiceClient.GetBlobContainerClient("invitations").GetBlobClient($"{invitationCode}.{fileType}");
            await client.UploadAsync(str, overwrite: true);
        }


        public async Task<byte[]> DownloadInvitationAsync(string invitationCode, string fileType)
        {
            byte[] theInvitationBytes;

            await using (MemoryStream memoryStream = new())
            {
                var client = _blobServiceClient
                        .GetBlobContainerClient("invitations")
                        .GetBlobClient($"{invitationCode}.{fileType}");

                await client.DownloadToAsync(memoryStream);
                theInvitationBytes = memoryStream.ToArray();
            }

            return theInvitationBytes;
        }

        public async Task<bool> FileExistsAsync(string invitationCode, string fileType)
        {
            return await _blobServiceClient
                .GetBlobContainerClient("invitations")
                .GetBlobClient($"{invitationCode}.{fileType}")
                .ExistsAsync();
        }

    }
}
