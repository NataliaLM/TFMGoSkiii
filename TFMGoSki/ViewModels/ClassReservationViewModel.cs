using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class ClassReservationViewModel
    {
        public int Id { get; set; }
        [Display(Name = "User")]
        [Required(ErrorMessage = "The user is required.")]
        public int UserId { get; set; }
        [Display(Name = "Class")]
        [Required(ErrorMessage = "The class is required.")]
        public int ClassId { get; set; }
        [Display(Name = "Reservation Time Range Class")]
        [Required(ErrorMessage = "The  Reservation Time Range Class is required.")]
        public int ReservationTimeRangeClassId { get; set; }
        [Display(Name = "Number of persons to be booked")]
        [Required(ErrorMessage = "The number of persons to be booked is mandatory.")]
        public int NumberPersonsBooked { get; set; }

        [Display(Name = "Class")]
        public string? ClassName { get; set; }
        [Display(Name = " Reservation Time Range Class")]
        public string? ReservationTimeRangeClassName { get; set; }
    }
}
