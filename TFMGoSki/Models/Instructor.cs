using Microsoft.EntityFrameworkCore.Update.Internal;

namespace TFMGoSki.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Instructor(string name)
        {
            Validate(name);

            Name = name;
        }

        public Instructor Update(string name)
        {
            Validate(name);
            Name = name;
            return this;
        }

        private void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("El nombre no puede ser nulo ni vacío.", nameof(name));
            }
        }
    }
}
