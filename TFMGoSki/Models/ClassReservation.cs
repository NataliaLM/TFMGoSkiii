namespace TFMGoSki.Models
{
    public class ClassReservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public ClassReservation(int userId, int classId)
        {
            UserId = userId;
            ClassId = classId;
        }
        public ClassReservation Update(int userId, int classId)
        {
            UserId = userId;
            ClassId = classId;
            return this;
        }
    }
}
