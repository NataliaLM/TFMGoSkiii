namespace TFMGoSki.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public City(string name)
        {
            Validate(name);

            Name = name;
        }

        public City Update(string name)
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
