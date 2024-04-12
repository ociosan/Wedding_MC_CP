using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UI.Models;
using UI.Singletons;

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

        [HttpGet("ShowFamilyMembers")]
        public async Task<IActionResult> ShowFamilyMembers(string invitationCode)
        {
            var result = await WebApiClientSingleton.GetInstance.GetOneByInvitationCodeAsync(invitationCode);
            return View(new FamilyModel(result));
        }

        [HttpPost("ConfirmAssistance")]
        public IActionResult ConfirmAssistance(string email, string invitationCode)
        {
            //await WebApiClientSingleton.GetInstance.ConfirmAssistanceAsync(email, invitationCode);

            return View("Index");
        }

    }
}