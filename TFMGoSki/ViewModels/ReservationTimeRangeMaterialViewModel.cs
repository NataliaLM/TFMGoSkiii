using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class ReservationTimeRangeMaterialViewModel : ReservationTimeRangeViewModel
    {
        [Display(Name = "Remaining Students Quantity")]
        [Required(ErrorMessage = "The remaining students quantity is required")]
        public int RemainingStudentsQuantity { get; set; }
        [Display(Name = "Material Name")]
        [Required(ErrorMessage = "The Material Name is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The Material must be greater than zero.")]
        public int Material { get; set; }
    }
}
