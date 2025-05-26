namespace TFMGoSki.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Raiting { get; set; } 
        public Comment(string text, int raiting)
        {
            Text = text;
            Raiting = raiting;
        }
        public Comment Update(string text, int raiting)
        {
            Text = text;
            Raiting = raiting;
            return this;
        }
    }
}
