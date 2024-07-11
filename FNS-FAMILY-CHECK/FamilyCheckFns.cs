using AutoMapper;
using Core.Interfaces.UnitOfWork;
using Data.Dto;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace FNS_FAMILY_CHECK
{
    public class FamilyCheckFns
    {
        private readonly ILogger<FamilyCheckFns> _logger;
        private readonly IWeddingDbUow _weddingDbUow;
        private readonly IMapper _mapper;

        public FamilyCheckFns(
            ILogger<FamilyCheckFns> logger, 
            IWeddingDbUow weddingDbUow, 
            IMapper mapper)
        {
            _logger = logger;
            _weddingDbUow = weddingDbUow;
            _mapper = mapper;
        }

        [Function("GetOneByInvitationCodeAsync")]
        [OpenApiOperation("GetOneByInvitationCodeAsync", nameof(ConfirmAssitanceDto), Description = "Get Family Members Information")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, [FromBody] ConfirmAssitanceDto confirmAssitanceDto)
        {
            try
            {
                Family family = await _weddingDbUow.Family.SelectOneRow($"SELECT TOP 1 * FROM dbo.Family WITH(NOLOCK) WHERE InvitationCode = '{confirmAssitanceDto.InvitationCode}'");
                if (family is not null)
                {
                    family.FamilyMembers = await _weddingDbUow.FamilyMember.GetList($"SELECT * FROM dbo.FamilyMember WITH(NOLOCK) WHERE FamilyId = {family.Id}");
                    return new OkObjectResult(_mapper.Map<FamilyDto>(family));
                }

                return new OkObjectResult(null);

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }


    }
}
