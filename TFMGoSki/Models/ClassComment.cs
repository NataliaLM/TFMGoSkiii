namespace TFMGoSki.Models
{
    public class ClassComment : Comment
    {
        public int ClassReservationId { get; set; }
        public ClassComment(int classReservationId, string text, int raiting) : base(text, raiting)
        {
            ClassReservationId = classReservationId;
            Text = text;
            Raiting = raiting;
        }
        public ClassComment Update(int classReservationId, string text, int raiting)
        {
            base.Update(text, raiting);
            ClassReservationId = classReservationId;
            return this;
        }
    }
}
