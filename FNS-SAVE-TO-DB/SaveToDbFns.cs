using System.Data.SqlClient;
using System.Text;
using Azure.Messaging.ServiceBus;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using Microsoft.Azure.Functions.Worker;
using Serilog;
using Newtonsoft.Json;
using Core.Interfaces.Service;
using Data.Entities;
using Core.Enum;

namespace FNS_SAVE_TO_DB
{
    public class SaveToDbFns
    {
        private readonly ILogger _logger;
        private readonly IWeddingDbUow _weddingDbUow;
        private readonly IAzureUow _azureUow;
        private readonly IConfirmAssistanceService _confirmAssistanceService;

        public SaveToDbFns(ILogger logger, IWeddingDbUow weddingDbUow, IAzureUow azureUow)
        {
            _logger = logger;
            _weddingDbUow = weddingDbUow;
            _azureUow = azureUow;
        }

        [Function(nameof(SaveToDbFns))]
        public async Task Run([ServiceBusTrigger("%QUEUE_NAME%", Connection = "SERVICE_BUS_CONNECTION")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            try
            {
                _logger.Information($"SAVE TO DB - Information Received: {Encoding.ASCII.GetString(message.Body)}");

                ConfirmObjectDto? incomingData = JsonConvert.DeserializeObject<ConfirmObjectDto>(Encoding.ASCII.GetString(message.Body));

                await _weddingDbUow.Family.Update($"UPDATE dbo.Family SET EmailAddress = '{incomingData.Email}', PhoneNumber = '{incomingData.PhoneNumber}' WHERE InvitationCode = '{incomingData.InvitationCode}';");

                Family familyDb = await _weddingDbUow.Family.SelectOneRow($"SELECT TOP 1 * FROM dbo.Family WITH(NOLOCK) WHERE InvitationCode = '{incomingData.InvitationCode}'");

                await _weddingDbUow.Email.InsertInto($"INSERT INTO dbo.Email (FamilyId, [To], Status, DateCreated) VALUES (@FamilyId, @To, @Status, @DateCreated)",
                    new Email() { FamilyId = familyDb.Id, To = incomingData.Email, Status = (int)EmailStatusEnum.Created, DateCreated = DateTime.UtcNow });

                await _weddingDbUow.WhatsApp.InsertInto($"INSERT INTO dbo.WhatsApp (FamilyId, PhoneNumber, Status, DateCreated) VALUES (@FamilyId, @PhoneNumber, @Status, @DateCreated)",
                    new WhatsApp() { FamilyId = familyDb.Id, PhoneNumber = incomingData.PhoneNumber, Status = (int)WhatsAppStatusEnum.Created, DateCreated = DateTime.UtcNow });

                await _azureUow.ServiceBus.SendMessageToQueueAsync("createinvitationpdf_queue", JsonConvert.SerializeObject(incomingData));


            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
            finally
            {
                await messageActions.CompleteMessageAsync(message);
            }
        }


        #region private
        private async Task SendToQueuesAsync(ConfirmObjectDto? incomingData)
        {
            /*await _azureUow.ServiceBus.SendMessageToQueueAsync("createinvitationpdf_queue", JsonConvert.SerializeObject(incomingData));
            await _azureUow.ServiceBus.SendMessageToQueueAsync("createinvitationjpg_queue", JsonConvert.SerializeObject(incomingData));
            await _azureUow.ServiceBus.SendMessageToQueueAsync("createemailtemplate_queue", JsonConvert.SerializeObject(incomingData));
            await _azureUow.ServiceBus.SendMessageToQueueAsync("createwhatsapptemplate_queue", JsonConvert.SerializeObject(incomingData));*/

        }
        #endregion
    }
}
