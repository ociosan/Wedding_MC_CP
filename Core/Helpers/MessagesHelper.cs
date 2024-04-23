using Core.Enum;
using Core.Interfaces.Helper;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using RestSharp;

namespace Core.Helpers
{
    public class MessagesHelper : IMessagesHelper
    {
        private readonly IAzureUow _azureUow;

        public MessagesHelper(IAzureUow azureUow)
        {
            _azureUow = azureUow;
        }

        public async Task SendWhatsappAsync(ConfirmAssitanceDto confirmAssitanceDto)
        {
            var url = $"https://api.ultramsg.com/{await _azureUow.KeyVault.GetSecretAsync(KeyVaultSecretsEnum.UltraMsgInstanceId)}/messages/image";
            var client = new RestClient(url);

            var request = new RestRequest(url, Method.Post);
            request.AddHeader("content-type", await _azureUow.KeyVault.GetSecretAsync(KeyVaultSecretsEnum.UltraMsgContentType));
            request.AddParameter("token", await _azureUow.KeyVault.GetSecretAsync(KeyVaultSecretsEnum.UltraMsgToken));
            request.AddParameter("to", $"+52{confirmAssitanceDto.PhoneNumber}");
            request.AddParameter("image", $"{await _azureUow.KeyVault.GetSecretAsync(KeyVaultSecretsEnum.ImageLocation)}{confirmAssitanceDto.InvitationCode}.{FileTypeEnum.Jpg}");
            request.AddParameter("caption", $"Nos encantará contar con tu valiosa asistencia. Atte: Mayra & Carlos");
            
            RestResponse response = await client.ExecuteAsync(request);
        }
    }
}
