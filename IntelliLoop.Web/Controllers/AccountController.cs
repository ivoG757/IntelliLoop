using Microsoft.AspNetCore.Mvc;

namespace IntelliLoop.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Login([FromForm] )
        //{
        //    return View();
        //}
    }
}
