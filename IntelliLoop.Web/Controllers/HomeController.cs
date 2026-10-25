using IntelliLoop.Web.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IntelliLoop.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            if(IsUserAuthenticated())
            {
                return RedirectToAction(nameof(ProcessingController.Upload), "Processing");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
