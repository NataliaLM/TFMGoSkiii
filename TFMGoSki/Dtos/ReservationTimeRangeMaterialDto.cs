using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class ReservationTimeRangeMaterialDto : ReservationTimeRangeDto
    {
        [Display(Name = "Remaining Materials Quantity")]
        public int RemainingMaterialsQuantity { get; set; }
        [Display(Name = "Material Name")]
        public string? MaterialId { get; set; }
    }
}
