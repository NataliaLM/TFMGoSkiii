using System;

namespace TFMGoSki.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StudentQuantity { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int InstructorId { get; set; }
        public int CityId { get; set; }

        public Class(string name, decimal price, int studentQuantity, ClassLevel classLevel, int instructorId, int cityId)
        {
            Validate(name, price, studentQuantity, classLevel, instructorId, cityId);

            Name = name;
            Price = price;
            StudentQuantity = studentQuantity;
            ClassLevel = classLevel;
            InstructorId = instructorId;
            CityId = cityId;
        }

        public Class Update(string name, decimal price, int studentQuantity, ClassLevel classLevel, int instructorId, int cityId)
        {
            Validate(name, price, studentQuantity, classLevel, instructorId, cityId);

            Name = name;
            Price = price;
            StudentQuantity = studentQuantity;
            ClassLevel = classLevel;
            InstructorId = instructorId;
            CityId = cityId;

            return this;
        }

        private void Validate(string name, decimal price, int studentQuantity, ClassLevel classLevel, int instructorId, int cityId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("El nombre no puede ser nulo ni vacío.", nameof(name));
            }
            if (price <= 0)
            {
                throw new ArgumentException("El precio debe ser mayor que cero.", nameof(price));
            }
            if (studentQuantity <= 0)
            {
                throw new ArgumentException("La cantidad de estudiantes debe ser mayor que cero.", nameof(studentQuantity));
            }
            if (!Enum.IsDefined(typeof(ClassLevel), classLevel))
            {
                throw new ArgumentException("El ID del nivel de clase debe ser mayor que cero.", nameof(classLevel));
            }
            if (instructorId <= 0)
            {
                throw new ArgumentException("El ID del instructor debe ser mayor que cero.", nameof(instructorId));
            }
            if (cityId <= 0)
            {
                throw new ArgumentException("El ID de la ciudad debe ser mayor que cero.", nameof(cityId));
            }
        }
    }
}
