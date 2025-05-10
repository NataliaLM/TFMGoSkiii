namespace TFMGoSki.Models
{
    public class Administrator : User
    {
        public Administrator(string name, string email, string phone, string password, string rol) : base(name, email, phone, password, rol)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Password = password;
            Rol = rol;
        }
        public Administrator Update(string name, string email, string phone, string password, string rol) 
        {
            base.Update(name, email, phone, password, rol);

            return this;
        }
    }
}
