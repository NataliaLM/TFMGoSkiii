using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class RegisterViewModel
    {
        public string Id { get; init; }
        [Required]
        public string FullName { get; init; } = default!;
        [Required, EmailAddress]
        public string Email { get; init; } = default!;
        [Required]
        public string PhoneNumber { get; init; } = default!;
        [Required, DataType(DataType.Password)]
        public string Password { get; init; } = default!;
        [DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; init; } = default!;
        [Required]
        public string RoleName { get; init; } = "Cliente";
    }
}
