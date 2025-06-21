using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "User Name")]
        [Required]
        public string UserName { get; init; } = default!;
        [Display(Name = "Password")]
        [Required, DataType(DataType.Password)]
        public string Password { get; init; } = default!;
        public bool RememberMe { get; init; }
    }
}
