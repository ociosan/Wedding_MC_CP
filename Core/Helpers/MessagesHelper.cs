using Core.Enum;
using Core.Interfaces.Helper;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using RestSharp;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

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
            request.AddParameter("caption", $"Nos encantará contar con tu valiosa asistencia. " +
                $" Presenta esta invitación en la entrada del salón de eventos. " +
                $" También esta misma invitación se envió al correo { confirmAssitanceDto.Email }" +
                $" Te esperamos para que seas testigo de éste gran amor." +
                $" Atte: Mayra & Carlos");
            
            await client.ExecuteAsync(request);
        }
    }
}
