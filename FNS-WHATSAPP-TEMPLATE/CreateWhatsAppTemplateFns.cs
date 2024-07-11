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

namespace FNS_WHATSAPP_TEMPLATE
{
    public class CreateWhatsAppTemplateFns
    {
        private readonly ILogger _logger;
        private readonly IFamilyRepository _familyRepository;
        private readonly IWeddingDbUow _weddingDbUow;

        public CreateWhatsAppTemplateFns(ILogger logger,
            IFamilyRepository familyRepository,
            IWeddingDbUow weddingDbUow)
        {
            _logger = logger;
            _familyRepository = familyRepository;
            _weddingDbUow = weddingDbUow;
        }

        [Function(nameof(CreateWhatsAppTemplateFns))]
        public async Task Run([ServiceBusTrigger("%QUEUE_NAME%", Connection = "SERVICE_BUS_CONNECTION")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {

            try
            {
                _logger.Information($"WHATSAPP TEMPLATE - Information Received: {Encoding.ASCII.GetString(message.Body)}");
                ConfirmObjectDto? incomingData = JsonConvert.DeserializeObject<ConfirmObjectDto>(Encoding.ASCII.GetString(message.Body));

                //Get Family from Db
                FamilyDto familyDto = await _familyRepository.GetOneByInvitationCodeAsync(incomingData.InvitationCode);

                /*await _weddingDbUow.WhatsApp.CreateAsync(new WhatsApp()
                {
                    FamilyId = familyDto.Id,
                    PhoneNumber = incomingData.PhoneNumber,
                    Status = (int)WhatsAppStatusEnum.Created,
                    DateCreated = DateTime.UtcNow
                });

                await _weddingDbUow.SaveAsync();*/

                _logger.Information($"WHATSAPP TEMPLATE - Invitation code {incomingData.InvitationCode} Succesfully Created");
            }
            catch (Exception ex)
            {
                _logger.Error($"WHATSAPP TEMPLATE: {ex}", ex.Message);
            }
            finally
            {
                // Complete the message
                await messageActions.CompleteMessageAsync(message);
            }
        }
    }
}
