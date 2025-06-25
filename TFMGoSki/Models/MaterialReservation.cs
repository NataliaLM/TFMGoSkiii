using Humanizer;

namespace TFMGoSki.Models
{
    public class MaterialReservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
        public bool Paid { get; set; }
        public MaterialReservation(int userId, decimal total, bool paid)
        {
            UserId = userId;
            Total = total;
            Paid = paid;
        }
        //public MaterialReservation Update(int userId, decimal total, bool paid)
        //{
        //    UserId = userId;
        //    Total = total;
        //    Paid = paid;
        //    return this;
        //}
        public MaterialReservation UpdatePaid(bool paid)
        {
            Paid = paid;
            return this;
        }
    }
}
