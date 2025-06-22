using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class MaterialCommentViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Reservation Material Cart Name")]
        public string? ReservationMaterialCartName { get; set; }
        [Display(Name = "ReservationMaterialCartId")]
        public int ReservationMaterialCartId { get; set; }
        [Display(Name = "Text")]
        [Required(ErrorMessage = "The text is required.")]
        [StringLength(100, ErrorMessage = "The text cannot exceed 100 characters.")]
        public string Text { get; set; }
        [Display(Name = "Rating")]
        [Required(ErrorMessage = "The raiting is required.")]
        [Range(1, 5, ErrorMessage = "The raiting must be greater than zero and less than five.")]
        public int Raiting { get; set; }
    }
}
