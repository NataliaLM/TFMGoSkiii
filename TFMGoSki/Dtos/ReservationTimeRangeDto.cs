using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class ReservationTimeRangeDto
    {
        public int Id { get; set; }        
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }
    }
}
