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
    }
}
