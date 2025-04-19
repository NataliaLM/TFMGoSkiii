namespace TFMGoSki.Models
{
    public class ReservationTimeRange
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ReservationTimeRange(DateTime startTime, DateTime endTime)
        {
            Validate(startTime, endTime);

            StartTime = startTime;
            EndTime = endTime;
        }

        public ReservationTimeRange Update(DateTime startTime, DateTime endTime)
        {
            Validate(startTime, endTime);

            StartTime = startTime;
            EndTime = endTime;

            return this;
        }

        private void Validate(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException("La hora de inicio debe ser anterior a la hora de fin.", nameof(startTime));
            }
        }
    }
}
