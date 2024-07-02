using System.Text;
using Azure.Messaging.ServiceBus;
using Core.Enum;
using Core.Interfaces.Repository;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
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
        private readonly IFamilyRepository _familyRepository;

        public CreatePdfFns(ILogger logger, 
            IAzureUow azureUow, 
            IHelpersUow helpersUow,
            IFamilyRepository familyRepository)
        {
            _logger = logger;
            _azureUow = azureUow;
            _helpersUow = helpersUow;
            _familyRepository = familyRepository;
        }

        [Function(nameof(CreatePdfFns))]
        public async Task Run([ServiceBusTrigger("%QUEUE_NAME%", Connection = "SERVICE_BUS_CONNECTION")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            try
            {
                _logger.Information($"Create Pdf - Information Received: {Encoding.ASCII.GetString(message.Body)}");

                ConfirmObjectDto? incomingData = JsonConvert.DeserializeObject<ConfirmObjectDto>(Encoding.ASCII.GetString(message.Body));

                FamilyDto familyDto = await _familyRepository.GetOneByInvitationCodeAsync(incomingData.InvitationCode);

                if (await _azureUow.StorageAccount.FileExistsAsync(incomingData.InvitationCode, FileTypeEnum.Pdf))
                {
                    StorageAccountMessageDto storageAccountMessageDto = new StorageAccountMessageDto() { subject = $"/blobServices/default/containers/invitations/blobs/{incomingData.InvitationCode}.{FileTypeEnum.Pdf}" };
                    await _azureUow.ServiceBus.SendMessageToQueueAsync("sendemail_queue", JsonConvert.SerializeObject(storageAccountMessageDto));
                }
                else
                {
                    await _helpersUow.Pdf.MakePDF(
                        familyDto.InvitationCode,
                        familyDto.LastName,
                        familyDto.FamilyMembers.Select(s => s.Names).ToList(),
                        await _azureUow.StorageAccount.DownloadInvitationTemplateAsync());
                }
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
