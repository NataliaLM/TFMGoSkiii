using Microsoft.AspNetCore.Identity;

namespace TFMGoSki.Models
{
    public class User : IdentityUser<int>
    {
        public string? FullName { get; set; }
    }
}
