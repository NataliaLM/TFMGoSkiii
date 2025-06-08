using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class ClassReservationDto
    {
        public int Id { get; set; }
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }
        [Display(Name = "Class Name")]
        public string ClassName { get; set; }
        public ReservationTimeRangeClassDto ReservationTimeRangeClassDto { get; set; }
    }
}
