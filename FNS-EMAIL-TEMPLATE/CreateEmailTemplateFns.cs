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

namespace FNS_EMAIL_TEMPLATE
{
    public class CreateEmailTemplateFns
    {
        private readonly ILogger _logger;
        private readonly IAzureUow _azureUow;
        private readonly IHelpersUow _helpersUow;
        private readonly IFamilyRepository _familyRepository;
        private readonly IWeddingDbUow _weddingDbUow;

        public CreateEmailTemplateFns(ILogger logger,
            IAzureUow azureUow,
            IHelpersUow helpersUow,
            IFamilyRepository familyRepository,
            IWeddingDbUow weddingDbUow)
        {
            _logger = logger;
            _azureUow = azureUow;
            _helpersUow = helpersUow;
            _familyRepository = familyRepository;
            _weddingDbUow = weddingDbUow;
        }

        [Function(nameof(CreateEmailTemplateFns))]
        public async Task Run([ServiceBusTrigger("%QUEUE_NAME%", Connection = "SERVICE_BUS_CONNECTION")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            try
            {
                _logger.Information($"Create Email Template - Information Received: {Encoding.ASCII.GetString(message.Body)}");
                ConfirmObjectDto? incomingData = JsonConvert.DeserializeObject<ConfirmObjectDto>(Encoding.ASCII.GetString(message.Body));

                //Get Family from Db
                FamilyDto familyDto = await _familyRepository.GetOneByInvitationCodeAsync(incomingData.InvitationCode);

                await _weddingDbUow.Email.CreateAsync(new Email() 
                {
                    FamilyId = familyDto.Id,
                    To = incomingData.Email,
                    Status = (int)EmailStatusEnum.Created,
                    DateCreated = DateTime.UtcNow
                });

                await _weddingDbUow.SaveAsync();

                _logger.Information($"Email Template for Invitation code {incomingData.InvitationCode} Succesfully Created");
            }
            catch (Exception ex)
            {
                _logger.Error($"Email Template: {ex}", ex.Message);
            }
            finally
            {
                // Complete the message
                await messageActions.CompleteMessageAsync(message);
            }
        }
    }
}
