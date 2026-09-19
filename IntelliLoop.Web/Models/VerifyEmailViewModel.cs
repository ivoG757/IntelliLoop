using System.ComponentModel.DataAnnotations;

namespace IntelliLoop.Web.Models
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
