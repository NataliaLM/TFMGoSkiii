using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class MaterialReservationViewModel
    {
        public int Id { get; set; }
        [Display(Name = "User")]
        public int UserId { get; set; }
        [Display(Name = "Total")]
        public int Total { get; set; }
        [Display(Name = "Paid")]
        public bool Paid { get; set; }
    }
}
