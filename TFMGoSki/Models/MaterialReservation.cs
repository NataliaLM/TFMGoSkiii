using Humanizer;

namespace TFMGoSki.Models
{
    public class MaterialReservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Total { get; set; }
        public MaterialReservation(int userId, int total)
        {
            UserId = userId;
            Total = total;
        }
        public MaterialReservation Update(int userId, int total)
        {
            UserId = userId;
            Total = total;
            return this;
        }
    }
}
