using Core.Interfaces.Repository;
using Data.Dto;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FamilyController : BaseApiController
    {
        private readonly IFamilyRepository _familyRepository;

        public FamilyController(IFamilyRepository familyRepository)
        {
            _familyRepository = familyRepository;
        }

        [HttpGet("GetAllByEmailAddresAsync/{emailAddres}")]
        public async Task<IActionResult> GetAllByEmailAddresAsync([FromRoute] string emailAddres)
        {
            return Ok(await _familyRepository.GetAllByEmailAddresAsync(emailAddres));
        }

        [HttpGet("GetAllByInvitationCodeAsync/{invitationCode}")]
        public async Task<IActionResult> GetAllByInvitationCodeAsync([FromRoute] string invitationCode)
        {
            return Ok(await _familyRepository.GetAllByInvitationCodeAsync(invitationCode));
        }

        [HttpPost("CreateFamilyAsync")]
        public async Task<IActionResult> CreateFamilyAsync([FromBody] NewFamilyDto familyDto)
        {
            await _familyRepository.CreateAsync(familyDto);
            return Ok();
        }

        [HttpPost("UpdateFamily")]
        public IActionResult UpdateFamily([FromBody] FamilyDto familyDto)
        {
            _familyRepository.Update(familyDto);
            return Ok();
        }
    }
}
