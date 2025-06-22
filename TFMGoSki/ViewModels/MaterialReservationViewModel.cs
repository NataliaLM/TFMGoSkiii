using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class MaterialReservationViewModel
    {
        public int Id { get; set; }
        [Display(Name = "User")]
        [Required(ErrorMessage = "The user is required.")]
        public int UserId { get; set; }
        [Display(Name = "Total")]
        [Required(ErrorMessage = "The Total is required.")]
        public int Total { get; set; }
    }
}
