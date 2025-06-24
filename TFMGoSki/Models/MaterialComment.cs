namespace TFMGoSki.Models
{
    public class MaterialComment : Comment
    {
        public int ReservationMaterialCartId { get; set; }
        public MaterialComment(int reservationMaterialCartId, string text, int raiting) : base(text, raiting)
        {
            ReservationMaterialCartId = reservationMaterialCartId;
            Text = text;
            Raiting = raiting;
        }
        public MaterialComment Update(int reservationMaterialCartId, string text, int raiting)
        {
            base.Update(text, raiting);
            ReservationMaterialCartId = reservationMaterialCartId;
            return this;
        }
    }
}
