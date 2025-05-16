using Microsoft.AspNetCore.Identity;

namespace TFMGoSki.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public User(string name, string email, string phone, string password, int roleId)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Password = password;
            RoleId = roleId;
        }
        public User Update(string name, string email, string phone, string password, int roleId)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Password = password;
            RoleId = roleId;

            return this;
        }
    }
}
