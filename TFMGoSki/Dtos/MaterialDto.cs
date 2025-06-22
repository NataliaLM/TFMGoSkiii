using System.ComponentModel.DataAnnotations;
using TFMGoSki.Models;

namespace TFMGoSki.Dtos
{
    public class MaterialDto
    {
        public int Id { get; set; }
        [Display(Name = "Material Name")]
        public string? Name { get; set; }
        [Display(Name = "Material Description")]
        public string? Description { get; set; }
        [Display(Name = "Material Quantity")]
        public int QuantityMaterial { get; set; }
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        [Display(Name = "Price")]
        public string? Size { get; set; }

        [Display(Name = "City Name")]
        public string? CityId { get; set; }
        [Display(Name = "Material Type")]
        public string? MaterialTypeId { get; set; }
        [Display(Name = "Material Status")]
        public string? MaterialStatusId { get; set; }
        public List<ReservationTimeRangeMaterialDto>? ReservationTimeRangeMaterialDto { get; set; }
        public List<MaterialCommentDto>? Comments { get; set; }
    }
}
