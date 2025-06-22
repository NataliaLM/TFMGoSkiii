using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class ReservationTimeRangeMaterialViewModel : ReservationTimeRangeViewModel
    {
        [Display(Name = "Remaining Materials Quantity")]
        [Required(ErrorMessage = "The remaining materials quantity is required")]
        public int RemainingMaterialsQuantity { get; set; }
        [Display(Name = "Material Name")]
        [Required(ErrorMessage = "The Material Name is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The Material must be greater than zero.")]
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
    }
}
