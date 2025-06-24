using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class MaterialTypeViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The name is required")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string? Name { get; set; }
    }
}
