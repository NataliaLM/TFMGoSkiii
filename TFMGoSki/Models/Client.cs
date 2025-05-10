namespace TFMGoSki.Models
{
    public class Client : User
    {
        public Client(string name, string email, string phone, string password, string rol)
            : base(name, email, phone, password, rol)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Password = password;
            Rol = rol;
        }

        public Client Update(string name, string email, string phone, string password, string rol)
        {
            base.Update(name, email, phone, password, rol);
            return this;
        }
    }
}
