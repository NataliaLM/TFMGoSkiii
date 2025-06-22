using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class MaterialTypeDto
    {
        public int Id { get; set; }
        [Display(Name = "Material Type Name")]
        public string? Name { get; set; }
    }
}
