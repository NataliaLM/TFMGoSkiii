using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.Dtos
{
    public class MaterialReservationDto
    {
        public int Id { get; set; }
        [Display(Name = "Client Name")]
        public string? ClientName { get; set; }
        [Display(Name = "Total")]
        public int Total { get; set; }
    }
}
