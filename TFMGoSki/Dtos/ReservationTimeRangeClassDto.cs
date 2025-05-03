using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class ReservationTimeRangeClassDto : ReservationTimeRangeDto
    {        
        [Display(Name = "Remaining Students Quantity")]
        public int RemainingStudentsQuantity { get; set; }
        [Display(Name = "Class Name")]
        public string Class { get; set; }
    }
}
