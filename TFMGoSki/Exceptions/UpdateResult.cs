namespace TFMGoSki.Exceptions
{
    public class UpdateResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public static UpdateResult Ok() => new() { Success = true };
        public static UpdateResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
}
