using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class MaterialStatusDto
    {
        public int Id { get; set; }
        [Display(Name = "Material Status Name")]
        public string? Name { get; set; }
    }
}
