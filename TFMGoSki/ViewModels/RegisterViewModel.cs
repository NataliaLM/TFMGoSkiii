using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Full Name")]
        [Required]
        public string FullName { get; init; } = default!;
        [Display(Name = "Email")]
        [Required, EmailAddress]
        public string Email { get; init; } = default!;
        [Display(Name = "Phone Number")]
        [Required, Phone]
        public string PhoneNumber { get; init; } = default!;
        [Display(Name = "Password")]
        [Required, DataType(DataType.Password)]
        public string Password { get; init; } = default!;
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; init; } = default!;
        [Display(Name = "Role Name")]
        [Required]
        public string RoleName { get; init; } = "Client";
    }
}
