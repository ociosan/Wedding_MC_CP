using System.Text;
using Azure.Messaging.ServiceBus;
using Core.Enum;
using Core.Interfaces.Repository;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Serilog;

namespace FNS_CREATE_PDF
{
    public class CreatePdfFns
    {
        private readonly ILogger _logger;
        private readonly IAzureUow _azureUow;
        private readonly IHelpersUow _helpersUow;
        private readonly IWeddingDbUow _weddingDbUow;

        public CreatePdfFns(ILogger logger, 
            IAzureUow azureUow, 
            IHelpersUow helpersUow,
            IWeddingDbUow weddingDbUow)
        {
            _logger = logger;
            _azureUow = azureUow;
            _helpersUow = helpersUow;
            _weddingDbUow = weddingDbUow;
        }

        [Function(nameof(CreatePdfFns))]
        public async Task Run([ServiceBusTrigger("%QUEUE_NAME%", Connection = "SERVICE_BUS_CONNECTION")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            try
            {
                _logger.Information($"CREATE PDF - Information Received: {Encoding.ASCII.GetString(message.Body)}");

                ConfirmObjectDto? incomingData = JsonConvert.DeserializeObject<ConfirmObjectDto>(Encoding.ASCII.GetString(message.Body));

                Family family = await _weddingDbUow.Family.SelectOneRow($"SELECT * FROM dbo.Family WITH(NOLOCK) WHERE InvitationCode = '{incomingData.InvitationCode}'");
                IEnumerable<FamilyMember> members = await _weddingDbUow.FamilyMember.GetList($"SELECT * FROM dbo.FamilyMember WITH(NOLOCK) WHERE FamilyId = '{family.Id}'");


                await _helpersUow.Pdf.MakePDF(
                    family.InvitationCode,
                    family.LastName,
                    members.Select(s => s.Names).ToList(),
                    await _azureUow.StorageAccount.DownloadInvitationTemplateAsync());

                _logger.Information($"CREATE PDF - {incomingData.InvitationCode}.{FileTypeEnum.Pdf} Succesfully Created");
            }
            catch (Exception ex) 
            {
                _logger.Error(ex, $"CREATE PDF - {ex.Message}");
            }
            finally
            {
                // Complete the message
                await messageActions.CompleteMessageAsync(message);
            }
        }
    }
}
