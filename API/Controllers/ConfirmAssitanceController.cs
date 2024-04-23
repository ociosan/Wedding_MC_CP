using Core.Interfaces.Service;
using Data.Dto;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ConfirmAssitanceController : BaseApiController
    {
        private readonly IConfirmAssistanceService _confirmAssistanceService;

        public ConfirmAssitanceController(IConfirmAssistanceService confirmAssistanceService)
        {
            _confirmAssistanceService = confirmAssistanceService;
        }

        [HttpPost("ConfirmAssistanceAsync")]
        public async Task<IActionResult> ConfirmAssistanceAsync([FromBody] ConfirmAssitanceDto confirmAssitanceDto)
        {
            await _confirmAssistanceService.ConfirmAssistanceAsync(confirmAssitanceDto);
            return Ok();
        }

        [HttpPost("ResendEmailAsync/{emailTo}/{invitationCode}")]
        public async Task<IActionResult> ResendEmailAsync([FromBody] ConfirmAssitanceDto confirmAssitanceDto)
        {
            await _confirmAssistanceService.ReSendEmailAsync(confirmAssitanceDto);
            return Ok();
        }
    }
}
