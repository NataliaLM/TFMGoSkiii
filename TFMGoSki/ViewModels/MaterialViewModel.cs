using System.ComponentModel.DataAnnotations;
using TFMGoSki.Models;

namespace TFMGoSki.ViewModels
{
    public class MaterialViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Material Name")]
        [Required(ErrorMessage = "The name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string? Name { get; set; }
        [Display(Name = "Material Description")]
        [Required(ErrorMessage = "The Description is required.")]
        [StringLength(100, ErrorMessage = "The Description cannot exceed 100 characters.")]
        public string? Description { get; set; }
        [Display(Name = "Material Quantity")]
        [Required(ErrorMessage = "The number of materials is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The number of materials must be greater than zero.")]
        public int? QuantityMaterial { get; set; }
        [Display(Name = "Price")]
        [Required(ErrorMessage = "The price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than zero.")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }
        [Display(Name = "Size")]
        [Required(ErrorMessage = "The Size is required.")]
        [StringLength(100, ErrorMessage = "The Size cannot exceed 100 characters.")]
        public string? Size { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "The city is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The city must be greater than zero.")]
        public int? CityId { get; set; }
        [Display(Name = "Material Type")]
        [Required(ErrorMessage = "The material type is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The material type must be greater than zero.")]
        public int? MaterialTypeId { get; set; }
        [Display(Name = "Material Status")]
        [Required(ErrorMessage = "The material status is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The material status must be greater than zero.")]
        public int? MaterialStatusId { get; set; }

    }
}
