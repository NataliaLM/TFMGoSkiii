using Microsoft.AspNetCore.Identity;

namespace TFMGoSki.Models
{
    public class Role : IdentityRole<int>
    {
        public Role() : base() { }
        public Role(string roleName) : base(roleName)
        {
        }
    }
}
