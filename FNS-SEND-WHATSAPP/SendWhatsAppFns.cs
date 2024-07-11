using Core.Enum;
using Core.Interfaces.UnitOfWork;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Serilog;

namespace FNS_SEND_WHATSAPP
{
    public class SendWhatsAppFns
    {
        private readonly ILogger _logger;
        private readonly IAzureUow _azureUow;
        private readonly IHelpersUow _helpersUow;
        private readonly IWeddingDbUow _weddingDbUow;

        public SendWhatsAppFns(ILogger logger,
            IAzureUow azureUow,
            IHelpersUow helpersUow,
            IWeddingDbUow weddingDbUow)
        {
            _logger = logger;
            _azureUow = azureUow;
            _helpersUow = helpersUow;
            _weddingDbUow = weddingDbUow;
        }

        [Function(nameof(SendWhatsAppFns))]
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer)
        {
            try
            {
                WhatsApp whatsApp = await _weddingDbUow.WhatsApp.SelectOneRow($"SELECT TOP 1 * FROM dbo.WhatsApp WITH(NOLOCK) WHERE Status = {(int)WhatsAppStatusEnum.Created}");
                if( whatsApp != null ) 
                {
                    Family family = await _weddingDbUow.Family.SelectOneRow($"SELECT TOP 1 * FROM dbo.Family WITH(NOLOCK) WHERE PhoneNumber = '{whatsApp.PhoneNumber}' AND Id = {whatsApp.FamilyId}");

                    //CHECK IF BLOB EXISTS
                    if(await _azureUow.StorageAccount.FileExistsAsync(family.InvitationCode,FileTypeEnum.Jpg))
                    {
                        await _helpersUow.Messages.SendWhatsappAsync(new Data.Dto.ConfirmAssitanceDto() { Email = family.EmailAddress, PhoneNumber = whatsApp.PhoneNumber, InvitationCode = family.InvitationCode });
                        await _weddingDbUow.WhatsApp.Update($"UPDATE dbo.WhatsApp SET Status = {(int)WhatsAppStatusEnum.Sent}, DateSent = GETDATE() WHERE Id = {whatsApp.Id}");
                        _logger.Information($"SEND WHATSAPP TO NUMBER: {whatsApp.PhoneNumber} successfully created");
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"SEND WHATSAPP - {ex.Message}");
            }
        }
    }
}
