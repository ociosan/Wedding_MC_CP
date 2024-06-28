using System.Data.SqlClient;
using System.Text;
using Azure.Messaging.ServiceBus;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Serilog;
using Newtonsoft.Json;

namespace FNS_SAVE_TO_DB
{
    public class SaveToDbFns
    {
        private readonly ILogger _logger;
        private readonly IWeddingDbUow _weddingDbUow;
        private readonly IAzureUow _azureUow;

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
                _logger.Information($"Information Received: {Encoding.ASCII.GetString(message.Body)}");

                ConfirmObjectDto? incomingData = JsonConvert.DeserializeObject<ConfirmObjectDto>(Encoding.ASCII.GetString(message.Body));

                await UpdateDatabaseAsync(incomingData);
                await SendToQueuesAsync(incomingData);

                //complete the message
                await messageActions.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                if (ex is SqlException)
                    await messageActions.DeferMessageAsync(message);
                else
                    await messageActions.CompleteMessageAsync(message);

            }
        }


        #region private
        private async Task SendToQueuesAsync(ConfirmObjectDto? incomingData)
        {
            await _azureUow.ServiceBus.SendMessageToQueueAsync("createinvitationpdf_queue", JsonConvert.SerializeObject(incomingData));
            await _azureUow.ServiceBus.SendMessageToQueueAsync("createemailtemplate_queue", JsonConvert.SerializeObject(incomingData));
            await _azureUow.ServiceBus.SendMessageToQueueAsync("createwhatsapptemplate_queue", JsonConvert.SerializeObject(incomingData));
        }

        private async Task UpdateDatabaseAsync(ConfirmObjectDto? incomingData)
        {
            //Get Info By  Invitation Code
            Family familyDb = await _weddingDbUow.Family.FindOneAsync(x => x.InvitationCode == incomingData.InvitationCode);

            familyDb.EmailAddress = incomingData.Email;
            familyDb.PhoneNumber = incomingData.PhoneNumber;

            //update 
            _weddingDbUow.Family.Update(familyDb);
            await _weddingDbUow.SaveAsync();
        }
        #endregion
    }
}
