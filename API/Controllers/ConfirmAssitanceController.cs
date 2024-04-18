using Core.Interfaces.Service;
using Microsoft.AspNetCore.Http;
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

        [HttpPost("ConfirmAssistanceAsync/{emailTo}/{invitationCode}")]
        public async Task<IActionResult> ConfirmAssistanceAsync(string emailTo, string invitationCode)
        {
            await _confirmAssistanceService.ConfirmAssistanceAsync(emailTo, invitationCode);
            return Ok();
        }

        [HttpPost("ResendEmailAsync/{emailTo}/{invitationCode}")]
        public async Task<IActionResult> ResendEmailAsync(string emailTo, string invitationCode)
        {
            await _confirmAssistanceService.ReSendEmailAsync(emailTo, invitationCode);  
            return Ok();
        }
    }
}
