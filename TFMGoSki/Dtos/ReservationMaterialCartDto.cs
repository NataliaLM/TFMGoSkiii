using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class ReservationMaterialCartDto
    {
        public int Id { get; set; }
        [Display(Name = "Material Name")]
        public string? MaterialName { get; set; }
        [Display(Name = "Material Reservation Name")]
        public string? MaterialReservationName { get; set; }
        [Display(Name = "User Name")]
        public string? UserName { get; set; }
        public ReservationTimeRangeMaterialDto? ReservationTimeRangeMaterialDto { get; set; }
        [Display(Name = "Numbre Materials Booked")]
        public int? NumberMaterialsBooked { get; set; }
    }
}
