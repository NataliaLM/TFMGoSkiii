namespace TFMGoSki.Models
{
    public enum ClassLevel
    {
        Beginner = 0,
        Intermediate = 1,
        Advanced = 2
    }

    public static class ClassLevelExtensions
    {
        public static string ToFriendlyString(this ClassLevel level)
        {
            return level switch
            {
                ClassLevel.Beginner => "Beginner",
                ClassLevel.Intermediate => "Intermediate",
                ClassLevel.Advanced => "Advanced",
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        }
    }
}
