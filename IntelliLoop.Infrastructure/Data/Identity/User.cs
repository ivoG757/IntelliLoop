using IntelliLoop.Core.Entities.Models;
using Microsoft.AspNetCore.Identity;
namespace IntelliLoop.Infrastructure.Data.Identity
{
    public class User : IdentityUser
    {
        public Lecture? Lecture { get; set; }
    }
}
