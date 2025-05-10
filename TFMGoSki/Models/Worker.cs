namespace TFMGoSki.Models
{
    public class Worker : User
    {
        public Worker(string name, string email, string phone, string password, string rol)
            : base(name, email, phone, password, rol)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Rol = rol;
        }

        public Worker Update(string name, string email, string phone, string password, string rol)
        {
            base.Update(name, email, phone, password, rol);
            return this;
        }
    }
}
