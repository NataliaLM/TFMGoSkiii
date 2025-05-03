using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class ReservationTimeRangeClassViewModel : ReservationTimeRangeViewModel
    {
        //[Display(Name = "Remaining Students Quantity")]
        //[Required(ErrorMessage = "The remaining students quantity is required")]
        //public int RemainingStudentsQuantity { get; set; }
        [Display(Name = "Class Name")]
        [Required(ErrorMessage = "The name is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The class ID must be greater than zero.")]
        public int Class { get; set; }
    }
}
