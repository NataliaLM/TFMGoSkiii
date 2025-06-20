using System.ComponentModel.DataAnnotations;
using TFMGoSki.Models;

namespace TFMGoSki.Dtos
{
    public class ClassDto
    {
        public int Id { get; set; }
        [Display(Name = "Class Name")]
        public string? Name { get; set; }
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        [Display(Name = "Student Quantity")]
        public int StudentQuantity { get; set; }
        [Display(Name = "Class Level")]
        public ClassLevel ClassLevel { get; set; }
        [Display(Name = "Instructor Name")]
        public string? InstructorName { get; set; }
        [Display(Name = "City Name")]
        public string? CityName { get; set; }
        public List<ReservationTimeRangeClassDto>? ReservationTimeRangeClassDto { get; set; }
        public List<ClassCommentDto> Comments { get; set; }
    }
}
