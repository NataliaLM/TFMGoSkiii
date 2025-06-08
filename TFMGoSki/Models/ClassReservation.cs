namespace TFMGoSki.Models
{
    public class ClassReservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int ReservationTimeRangeClassId { get; set; }
        public ClassReservation(int userId, int classId, int rervationTimeRangeClassId)
        {
            UserId = userId;
            ClassId = classId;
            ReservationTimeRangeClassId = ReservationTimeRangeClassId;
        }
        public ClassReservation Update(int userId, int classId, int reservationTimeRangeClassId)
        {
            UserId = userId;
            ClassId = classId;
            ReservationTimeRangeClassId = reservationTimeRangeClassId;
            return this;
        }
    }
}
