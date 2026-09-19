using Microsoft.AspNetCore.Identity;

namespace IntelliLoop.Web.Identity
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = null!;
    }
}
