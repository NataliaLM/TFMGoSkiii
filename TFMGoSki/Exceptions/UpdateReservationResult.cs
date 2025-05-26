namespace TFMGoSki.Exceptions
{
    public class UpdateReservationResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public static UpdateReservationResult Ok() => new() { Success = true };
        public static UpdateReservationResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
}
