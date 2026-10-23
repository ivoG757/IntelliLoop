using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliLoop.Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected Guid GetUserId() 
            => IsUserAuthenticated() == true ? 
            Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty) : Guid.Empty;
        protected bool IsUserAuthenticated()
            => User?.Identity?.IsAuthenticated == true;
    }
}
