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

        public async Task UploadInvitationCodeAsync(Stream fileContent, string invitationCode)
        {
            await using (Stream str = fileContent)
            {
                var client = _blobServiceClient.GetBlobContainerClient("invitations").GetBlobClient($"{invitationCode}.pdf");
                await client.UploadAsync(str, overwrite: true);
            }
        }

        //hacer método para descargar bytes del pdf creado 

    }
}
