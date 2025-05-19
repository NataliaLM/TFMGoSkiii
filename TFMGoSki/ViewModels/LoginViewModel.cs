using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class LoginViewModel
    {
        public string Id { get; init; } = default!;
        [Required]
        public string UserName { get; init; } = default!;
        [Required, DataType(DataType.Password)]
        public string Password { get; init; } = default!;
        public bool RememberMe { get; init; }
    }
}
