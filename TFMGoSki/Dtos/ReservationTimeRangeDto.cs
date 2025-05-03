using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class ReservationTimeRangeDto
    {
        public int Id { get; set; }        
        [Display(Name = "Start Time")]
        public TimeOnly StartTimeOnly { get; set; }
        [Display(Name = "End Time")]
        public TimeOnly EndTimeOnly { get; set; }
        [Display(Name = "Start Date")]
        public DateOnly StartDateOnly { get; set; }
        [Display(Name = "End Date")]
        public DateOnly EndDateOnly { get; set; }
    }
}
