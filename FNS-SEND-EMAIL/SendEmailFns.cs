using Azure.Messaging.ServiceBus;
using Core.Enum;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace FNS_SEND_EMAIL
{
    public class SendEmailFns
    {
        private readonly ILogger _logger;
        private readonly IAzureUow _azureUow;
        private readonly IHelpersUow _helpersUow;
        private readonly IWeddingDbUow _weddingDbUow;


        public SendEmailFns(ILogger logger,
            IAzureUow azureUow,
            IHelpersUow helpersUow,
            IWeddingDbUow weddingDbUow)
        {
            _logger = logger;
            _azureUow = azureUow;
            _helpersUow = helpersUow;
            _weddingDbUow = weddingDbUow;
        }

        [Function(nameof(SendEmailFns))]
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer)
        {
            try
            {
                try
                {
                    Email email = await _weddingDbUow.Email.SelectOneRow($"SELECT TOP 1 * FROM dbo.Email WITH(NOLOCK) WHERE Status= {(int)EmailStatusEnum.Created} AND DateSent IS NULL");
                    if(email != null)
                    {
                        Family family = await _weddingDbUow.Family.SelectOneRow($"SELECT TOP 1 * FROM dbo.Family WITH(NOLOCK) WHERE EmailAddress = '{email.To}' AND Id = {email.FamilyId}");
                        //CHECK IF BLOB EXISTS
                        if (await _azureUow.StorageAccount.FileExistsAsync(family.InvitationCode, FileTypeEnum.Jpg))
                        {
                            await _helpersUow.Email.SendEmailAsync(new MailRequestDto(
                                toEmail: email.To,
                                subject: "Nuestra Boda - Mayra & Carlos",
                                body: "<html><body><img src=\"cid:image1\"></body></html>",
                                await _helpersUow.Pdf.ConvertPdfToImage($"{family.InvitationCode}")));

                            await _weddingDbUow.Email.Update($"UPDATE dbo.Email SET Status = {(int)EmailStatusEnum.Sent}, DateSent = GETDATE() WHERE Id = {email.Id}");
                            _logger.Information($"SEND EMAIL TO: {email.To} successfully sent");
                        }
                    }
                }
                catch (Exception ex) 
                {
                    _logger.Error(ex, $"SEND EMAIL - {ex.Message}");
                }
            }
            catch(Exception ex) 
            {
                _logger.Error(ex, $"SEND EMAIL: {ex.Message}");
            }
        }
    }
}
