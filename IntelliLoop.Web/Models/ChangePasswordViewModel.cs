using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static IntelliLoop.Web.Common.Constants.Account;
namespace IntelliLoop.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(maximumLength: MaximumPasswordLength, MinimumLength = MinimumPasswordLength)]
        [DataType(DataType.Password)]
        [Compare(nameof(ConfirmNewPassword), ErrorMessage = "Password does not match.")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Password confirmation is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
