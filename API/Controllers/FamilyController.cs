using Core.Interfaces.Repository;
using Data.Dto;
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
        private async Task<ActionResult<List<FamilyDto>>> GetAllByEmailAddresAsync([FromRoute] string emailAddres)
        {
            return Ok(await _familyRepository.GetAllByEmailAddresAsync(emailAddres));
        }

        [HttpGet("GetAllByInvitationCodeAsync/{invitationCode}")]
        private async Task<ActionResult<List<FamilyDto>>> GetAllByInvitationCodeAsync([FromRoute] string invitationCode)
        {
            return Ok(await _familyRepository.GetAllByInvitationCodeAsync(invitationCode));
        }

        [HttpPost("CreateFamilyAsync")]
        private async Task<IActionResult> CreateFamilyAsync([FromBody] NewFamilyDto familyDto)
        {
            await _familyRepository.CreateAsync(familyDto);
            return Ok();
        }

        [HttpPost("UpdateFamily")]
        private IActionResult UpdateFamily([FromBody] FamilyDto familyDto)
        {
            _familyRepository.Update(familyDto);
            return Ok();
        }

        [HttpGet("GetOneByInvitationCodeAsync/{invitationCode}")]
        public async Task<ActionResult<FamilyDto>> GetOneByInvitationCodeAsync([FromRoute]string invitationCode)
        {
            return Ok(await _familyRepository.GetOneByInvitationCodeAsync(invitationCode));
        }

    }
}
