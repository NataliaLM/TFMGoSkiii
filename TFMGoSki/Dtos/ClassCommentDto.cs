using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class ClassCommentDto
    {
        public int Id { get; set; }
        [Display(Name = "Class Reservation Name")]
        public string ClassReservationName { get; set; }
        [Display(Name = "Text")]
        public string Text { get; set; }
        [Display(Name = "Raiting")]
        public int Raiting { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}
