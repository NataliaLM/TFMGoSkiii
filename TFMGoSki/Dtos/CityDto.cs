using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class CityDto
    {
        public int Id { get; set; }
        [Display(Name = "City Name")]
        public string? Name { get; set; }
    }
}
