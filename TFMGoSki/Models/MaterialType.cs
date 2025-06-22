using Microsoft.EntityFrameworkCore.Update.Internal;

namespace TFMGoSki.Models
{
    public class MaterialType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public MaterialType(string name)
        {
            Validate(name);

            Name = name;
        }

        public MaterialType Update(string name)
        {
            Validate(name);
            Name = name;
            return this;
        }

        private void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name cannot be null or empty.", nameof(name));
            }
        }
    }
}
