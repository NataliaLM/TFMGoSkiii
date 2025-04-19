using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class InstructorDto
    {
        public int Id { get; set; }
        [Display(Name = "Instructor Name")]
        public string? Name { get; set; }
    }
}
