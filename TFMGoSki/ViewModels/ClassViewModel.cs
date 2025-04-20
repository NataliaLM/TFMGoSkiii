using System.ComponentModel.DataAnnotations;
using TFMGoSki.Models;

namespace TFMGoSki.ViewModels
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Class Name")]
        [Required(ErrorMessage = "The name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string? Name { get; set; }
        [Display(Name = "Price")]
        [Required(ErrorMessage = "The price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than zero.")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }
        [Display(Name = "Student Quantity")]
        [Required(ErrorMessage = "The number of students is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The number of students must be greater than zero.")]
        public int? StudentQuantity { get; set; }
        [Display(Name = "Class Level")]
        [Required(ErrorMessage = "The class level is required.")]
        [EnumDataType(typeof(ClassLevel), ErrorMessage = "The class level is not valid.")]
        public ClassLevel? ClassLevel { get; set; }
        [Display(Name = "Instructor")]
        [Required(ErrorMessage = "The instructor ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The instructor ID must be greater than zero.")]
        public int? Instructor { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "The city ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The city ID must be greater than zero.")]
        public int? City { get; set; }
    }
}
