using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class ReservationTimeRangeMaterialDto : ReservationTimeRangeDto
    {
        [Display(Name = "Remaining Students Quantity")]
        public int RemainingStudentsQuantity { get; set; }
        [Display(Name = "Material Name")]
        public string? MaterialId { get; set; }
    }
}
