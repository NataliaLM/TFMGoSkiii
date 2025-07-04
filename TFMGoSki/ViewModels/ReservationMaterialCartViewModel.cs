using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class ReservationMaterialCartViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Material")]
        [Required(ErrorMessage = "The Material is required.")]
        public int MaterialId { get; set; }
        [Display(Name = "Material Name")]
        public string? MaterialName { get; set; }
        [Display(Name = "Material Reservation")]
        [Required(ErrorMessage = "The Material Reservation is required.")]
        public int MaterialReservationId { get; set; }
        [Display(Name = "Material Reservation")]
        public string? MaterialReservationName { get; set; }
        [Display(Name = "User")]
        [Required(ErrorMessage = "The user is required.")]
        public int UserId { get; set; }
        [Display(Name = "User")]
        public string? UserName { get; set; }
        [Display(Name = "Reservation Time Range Material")]
        [Required(ErrorMessage = "The  Reservation Time Range Material is required.")]
        public int ReservationTimeRangeMaterialId { get; set; }
        [Display(Name = "Reservation Time Range Material")]
        public string? ReservationTimeRangeMaterialName { get; set; }
        [Display(Name = "Number Materials Booked")]
        [Required(ErrorMessage = "The Number of Materials Booked is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The number of materials booked must be greater than zero.")]
        public int NumberMaterialsBooked { get; set; }

    }
}
