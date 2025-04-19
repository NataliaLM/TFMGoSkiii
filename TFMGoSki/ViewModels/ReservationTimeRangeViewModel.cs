using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class ReservationTimeRangeViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "The start time is required")]
        public DateTime StartTime { get; set; }
        [Display(Name = "End Time")]
        [Required(ErrorMessage = "The end time is required")]
        public DateTime EndTime { get; set; }
    }
}
