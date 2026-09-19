using System.ComponentModel.DataAnnotations;
using static IntelliLoop.Web.Common.Constants.Account;
namespace IntelliLoop.Web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(maximumLength: MaximumPasswordLength, MinimumLength = MinimumPasswordLength)]
        [DataType(DataType.Password)]
        [Compare(nameof(ConfirmPassword), ErrorMessage = "Password does not match.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Password confirmation is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
