using Core.Enum;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Serilog;

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
               string bodyInQueue = await _azureUow.ServiceBus.ReadMessageFromQueueAsync(Environment.GetEnvironmentVariable("QUEUE_NAME"));
               if(!string.IsNullOrEmpty(bodyInQueue))
               {
                    // También checar que el pdf ya exista en el Blob Storage 
                    ConfirmObjectDto incomingData = JsonConvert.DeserializeObject<ConfirmObjectDto>(bodyInQueue);
                    Email emailToSend = await _weddingDbUow.Email.FindOneAsync(x => 
                        x.To == incomingData.Email 
                        && x.Status == (int)EmailStatusEnum.Created 
                        && x.DateSent == null);

                    if(emailToSend is not null)
                    {
                        await _helpersUow.Email.SendEmailAsync(new MailRequestDto(
                            toEmail: incomingData.Email,
                            subject: "Nuestra Boda - Mayra & Carlos",
                            body: "<html><body><img src=\"cid:image1\"></body></html>",
                            await _helpersUow.Pdf.ConvertPdfToImage(incomingData.InvitationCode)));

                        emailToSend.Status = (int)EmailStatusEnum.Sent;
                        emailToSend.DateSent = DateTime.UtcNow;

                        _weddingDbUow.Email.Update(emailToSend);
                        _weddingDbUow.Save();

                        _logger.Information($"Invitation Sent to: {incomingData.Email} with Invitation #: {incomingData.InvitationCode} ");
                    }
               }
            }
            catch(Exception ex) 
            {
                _logger.Error(ex, $"Send Email: {ex.Message}");
            }
        }
    }
}
