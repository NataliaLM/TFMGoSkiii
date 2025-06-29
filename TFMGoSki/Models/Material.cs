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
        public string Size { get; set; }
        public int CityId { get; set; }
        public int MaterialTypeId { get; set; }
        public int MaterialStatusId { get; set; }

        public Material(string name, string description, int quantityMaterial, decimal price, string size, int cityId, int materialTypeId, int materialStatusId)
        { 
            Name = name;
            Description = description;
            QuantityMaterial = quantityMaterial;
            Price = price;
            Size = size;
            CityId = cityId;
            MaterialTypeId = materialTypeId;
            MaterialStatusId = materialStatusId;
        }

        public Material Update(string name, string description, int quantityMaterial, decimal price, string size, int cityId, int materialTypeId, int materialStatusId)
        { 
            Name = name;
            Description = description;
            QuantityMaterial = quantityMaterial;
            Price = price;
            Size = size;
            CityId = cityId;
            MaterialTypeId = materialTypeId;
            MaterialStatusId = materialStatusId;
            return this;
        }
         
    }
}
