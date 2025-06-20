using Humanizer;

namespace TFMGoSki.Models
{
    public class ClassReservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int ReservationTimeRangeClassId { get; set; }
        public int NumberPersonsBooked { get; set; }
        public ClassReservation(int userId, int classId, int reservationTimeRangeClassId, int numberPersonsBooked)
        {
            UserId = userId;
            ClassId = classId;
            ReservationTimeRangeClassId = reservationTimeRangeClassId;
            NumberPersonsBooked = numberPersonsBooked;
        }
        public ClassReservation Update(int userId, int classId, int reservationTimeRangeClassId, int numberPersonsBooked)
        {
            UserId = userId;
            ClassId = classId;
            ReservationTimeRangeClassId = reservationTimeRangeClassId;
            NumberPersonsBooked = numberPersonsBooked;
            return this;
        }
    }
}
