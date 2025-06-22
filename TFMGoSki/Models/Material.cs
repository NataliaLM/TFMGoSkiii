using System;

namespace TFMGoSki.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int QuantityMaterial { get; set; }
        public decimal Price {  get; set; }
        public int CityId { get; set; }
        public int MaterialTypeId { get; set; }
        public int MaterialStatusId { get; set; }

        public Material(string name, string description, int quantityMaterial, decimal price, int cityId, int materialTypeId, int materialStatusId)
        {
            Validate(name, description, quantityMaterial, price, cityId, materialTypeId, materialStatusId);

            Name = name;
            Description = description;
            QuantityMaterial = quantityMaterial;
            Price = price;
            CityId = cityId;
            MaterialTypeId = materialTypeId;
            MaterialStatusId = materialStatusId;
        }

        public Material Update(string name, string description, int quantityMaterial, decimal price, int cityId, int materialTypeId, int materialStatusId)
        {
            Validate(name, description, quantityMaterial, price, cityId, materialTypeId, materialStatusId);
            Name = name;
            Description = description;
            QuantityMaterial = quantityMaterial;
            Price = price;
            CityId = cityId;
            MaterialTypeId = materialTypeId;
            MaterialStatusId = materialStatusId;
            return this;
        }

        private void Validate(string name, string description, int quantityMaterial, decimal price, int cityId, int materialTypeId, int materialStatusId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name cannot be null or empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("The description cannot be null or empty.", nameof(description));
            }
            if (quantityMaterial <= 0)
            {
                throw new ArgumentException("The quantityMaterial cannot be less than zero.", nameof(quantityMaterial));
            }
            if (price <= 0)
            {
                throw new ArgumentException("The price cannot be less than zero.", nameof(price));
            }
            if (cityId <= 0)
            {
                throw new ArgumentException("The cityId cannot be less than zero.", nameof(cityId));
            }
            if (materialTypeId <= 0)
            {
                throw new ArgumentException("The materialTypeId cannot be less than zero.", nameof(materialTypeId));
            }
            if (materialStatusId <= 0)
            {
                throw new ArgumentException("The materialStatusId cannot be less than zero.", nameof(materialStatusId));
            }
        }
    }
}
