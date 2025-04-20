using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class ReservationTimeRangeViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "The start date is required")]
        [DataType(DataType.Date)]
        public DateOnly StartDateOnly { get; set; }
        [Display(Name = "End Date")]
        [Required(ErrorMessage = "The end date is required")]
        [DataType(DataType.Date)]
        public DateOnly EndDateOnly { get; set; }
        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "The start time is required")]
        [DataType(DataType.Time)]
        public TimeOnly StartTimeOnly { get; set; }
        [Display(Name = "End Time")]
        [Required(ErrorMessage = "The end time is required")]
        [DataType(DataType.Time)]
        public TimeOnly EndTimeOnly { get; set; }
    }
}
