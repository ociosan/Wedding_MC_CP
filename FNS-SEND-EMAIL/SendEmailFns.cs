using Azure.Messaging.ServiceBus;
using Core.Enum;
using Core.Interfaces.Repository;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using Data.Entities;
using iTextSharp.text;
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
        public async Task Run([ServiceBusTrigger("%QUEUE_NAME%", Connection = "SERVICE_BUS_CONNECTION")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            try
            {
                StorageAccountMessageDto accountMessageDto = JsonConvert.DeserializeObject<StorageAccountMessageDto>(Encoding.ASCII.GetString(message.Body));
                string incomingFileName = accountMessageDto.subject
                    .Split('/')
                    .FirstOrDefault(fod => fod.Contains($".{FileTypeEnum.Pdf}"))
                    .Split('.')
                    .First();

                Family family = await _weddingDbUow.Family.FindOneAsync(c => c.InvitationCode == incomingFileName);
                Email emailToSend = await _weddingDbUow.Email.FindOneAsync(x =>
                    x.To == family.EmailAddress
                    && x.Status == (int)EmailStatusEnum.Created
                    && x.DateSent == null);

                if (emailToSend is not null)
                {
                    await _helpersUow.Email.SendEmailAsync(new MailRequestDto(
                        toEmail: family.EmailAddress,
                        subject: "Nuestra Boda - Mayra & Carlos",
                        body: "<html><body><img src=\"cid:image1\"></body></html>",
                        await _helpersUow.Pdf.ConvertPdfToImage($"{incomingFileName}")));

                    emailToSend.Status = (int)EmailStatusEnum.Sent;
                    emailToSend.DateSent = DateTime.UtcNow;

                    _weddingDbUow.Email.Update(emailToSend);
                    _weddingDbUow.Save();

                    _logger.Information($"Invitation Sent to: {family.EmailAddress} with Invitation #: {incomingFileName} ");
                }

                await messageActions.CompleteMessageAsync(message);
            }
            catch(Exception ex) 
            {
                _logger.Error(ex, $"Send Email: {ex.Message}");
            }
        }
    }
}
