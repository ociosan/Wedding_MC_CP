using Azure.Messaging.ServiceBus;
using Core.Enum;
using Core.Interfaces.Repository;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace FNS_CREATE_JPG
{
    public class CreateJpgFns
    {
        private readonly ILogger _logger;
        private readonly IAzureUow _azureUow;
        private readonly IHelpersUow _helpersUow;

        public CreateJpgFns(ILogger logger,
            IAzureUow azureUow,
            IHelpersUow helpersUow)
        {
            _logger = logger;
            _azureUow = azureUow;
            _helpersUow = helpersUow;
        }

        [Function(nameof(CreateJpgFns))]
        public async Task Run([ServiceBusTrigger("%QUEUE_NAME%", Connection = "SERVICE_BUS_CONNECTION")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            try
            {
                _logger.Information($"CREATE JPG - Information Received: {Encoding.ASCII.GetString(message.Body)}");
                //ConfirmObjectDto? incomingData = JsonConvert.DeserializeObject<ConfirmObjectDto>(Encoding.ASCII.GetString(message.Body));

                StorageAccountMessageDto accountMessageDto = JsonConvert.DeserializeObject<StorageAccountMessageDto>(Encoding.ASCII.GetString(message.Body));
                string incomingFileName = accountMessageDto.subject
                    .Split('/')
                    .FirstOrDefault(fod => fod.Contains($".{FileTypeEnum.Pdf}"))
                    .Split('.')
                    .First();

                byte[] jpgFile = await _helpersUow.Pdf.ConvertPdfToImage(incomingFileName);

                await _azureUow.StorageAccount.UploadInvitationCodeAsync(new MemoryStream(jpgFile), incomingFileName, FileTypeEnum.Jpg);
                _logger.Information($"CREATE JPG - File: {incomingFileName}.{FileTypeEnum.Jpg} successfully created");

            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"CREATE JPG - {ex.Message}");
            }
            finally
            {
                // Complete the message
                await messageActions.CompleteMessageAsync(message);
            }
        }
    }
}
