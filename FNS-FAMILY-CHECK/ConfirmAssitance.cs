using Core.Interfaces.UnitOfWork;
using Data.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace FNS_FAMILY_CHECK
{
    public class ConfirmAssitance
    {
        private readonly ILogger<ConfirmAssitance> _logger;
        private readonly IAzureUow _azureUow;

        public ConfirmAssitance(ILogger<ConfirmAssitance> logger, IAzureUow azureUow)
        {
            _logger = logger;
            _azureUow = azureUow;
        }

        [Function("ConfirmAssitanceAsync")]
        [OpenApiOperation("ConfirmAssitanceAsync", nameof(ConfirmAssitanceDto), Description = "Generate WhatsApp and Email for the given Invitation Code")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, [FromBody] ConfirmAssitanceDto confirmAssitanceDto)
        {
            try
            {
                await _azureUow.ServiceBus.SendMessageToQueueAsync("fromconfirmationapimqueue ",
                    JsonConvert.SerializeObject(new ConfirmObjectDto()
                    {
                        Email = confirmAssitanceDto.Email,
                        PhoneNumber = confirmAssitanceDto.PhoneNumber,
                        InvitationCode = confirmAssitanceDto.InvitationCode
                    }));

                return new OkObjectResult(string.Empty);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
