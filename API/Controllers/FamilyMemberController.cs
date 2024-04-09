using Core.Interfaces.Repository;
using Data.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FamilyMemberController : BaseApiController
    {
        private readonly IFamilyMemberRepository _familyMemberRepository;

        public FamilyMemberController(IFamilyMemberRepository familyMemberRepository)
        {
            _familyMemberRepository = familyMemberRepository;
        }

        [HttpPut("AddFamilyMemberAsync")]
        public async Task<IActionResult> AddFamilyMemberAsync([FromBody] FamilyMemberDto familyMemberDto)
        {
            await _familyMemberRepository.AddFamilyMemberAsync(familyMemberDto);
            return Ok();
        }

        [HttpDelete("DeleteFamilyMember")]
        public IActionResult DeleteFamilyMember([FromBody] FamilyMemberDto familyMemberDto)
        {
            _familyMemberRepository.Delete(familyMemberDto);
            return Ok();
        }

    }
}
