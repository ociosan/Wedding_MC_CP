using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UI.Models;
using UI.Singletons;
using Wedding.Api;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ActionResult ReturnToIndex()
        {
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("ShowMembers/{invitationcode}")]
        public IActionResult ShowMembers(string invitationCodeToConfirm)
        {
            if (string.IsNullOrEmpty(invitationCodeToConfirm))
                return View();

            return Json(new { redirectToUrl = $"ShowFamilyMembers?invitationcode={invitationCodeToConfirm}" });
        }

        [HttpGet("ShowFamilyMembers")]
        public async Task<IActionResult> ShowFamilyMembers([FromQuery] string invitationcode)
        {
            var result = await WebApiClientSingleton.GetInstance.GetOneByInvitationCodeAsync(invitationcode);

            if(result != null)
                return View(new FamilyModel(result));

            return Json(new { redirectToUrl = "/" });
        }

        [HttpPost("ConfirmAssistance/{email}/{invitationCode}/{phoneNumber}")]
        public async Task<IActionResult> ConfirmAssistance(string email, string invitationCode, string phoneNumber)
        {
            if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(phoneNumber))
            {
                await WebApiClientSingleton.GetInstance.ConfirmAssistanceAsync(new ConfirmAssitanceDto() { Email = email, InvitationCode = invitationCode, PhoneNumber = phoneNumber });
                return Json(new { redirectToUrl = "Confirmation" });
            }

            return Json(new { redirectToUrl = $"ShowFamilyMembers?invitationcode={invitationCode}" });
        }

        [HttpGet("Confirmation")]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}