using Data.Dto;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using UI.Models;
using UI.Singletons;
using Wedding.Api;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(new ConfirmAssitanceDto() { InvitationCode = invitationcode.ToUpper().Trim() });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://fns-family-check.azurewebsites.net/api/GetOneByInvitationCodeAsync", content);
                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(await response.Content.ReadAsStringAsync()))
                    {
                        var familyDto = JsonConvert.DeserializeObject<FamilyDto>(await response.Content.ReadAsStringAsync());
                        return View(new FamilyModel(familyDto));
                    }
                    else
                        return View("Index");
                        //return Json(new { redirectToUrl = "/" });
                }
            }
            //var result = await WebApiClientSingleton.GetInstance.GetOneByInvitationCodeAsync(invitationcode);

            /*if (result != null)
                return View(new FamilyModel(result));*/
            return Json(new { redirectToUrl = "/" });
        }

        [HttpPost("ConfirmAssistance/{email}/{invitationCode}/{phoneNumber}")]
        public async Task<IActionResult> ConfirmAssistance(string email, string invitationCode, string phoneNumber)
        {
            if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(phoneNumber))
            {
                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(new ConfirmAssitanceDto() { Email = email, InvitationCode = invitationCode.ToUpper().Trim(), PhoneNumber = phoneNumber });
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://fns-family-check.azurewebsites.net/api/ConfirmAssitanceAsync", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                    }
                }

                //await WebApiClientSingleton.GetInstance.ConfirmAssitanceAsync(new ConfirmAssitanceDto() { Email = email, InvitationCode = invitationCode, PhoneNumber = phoneNumber });
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