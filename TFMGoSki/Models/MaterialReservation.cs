using Humanizer;

namespace TFMGoSki.Models
{
    public class MaterialReservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Total { get; set; }
        public bool Paid { get; set; }
        public MaterialReservation(int userId, int total, bool paid)
        {
            UserId = userId;
            Total = total;
            Paid = paid;
        }
        public MaterialReservation Update(int userId, int total, bool paid)
        {
            UserId = userId;
            Total = total;
            Paid = paid;
            return this;
        }
    }
}
