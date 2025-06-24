using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class MaterialCommentDto
    {
        public int Id { get; set; }
        [Display(Name = "Text")]
        public string? Text { get; set; }
        [Display(Name = "Raiting")]
        public int Raiting { get; set; }
        [Display(Name = "Reservation Material Cart Name")]
        public string? ReservationMaterialCartName { get; set; }
    }
}
