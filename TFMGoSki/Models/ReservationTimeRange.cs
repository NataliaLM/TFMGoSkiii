namespace TFMGoSki.Models
{
    public class ReservationTimeRange
    {
        public int Id { get; set; }
        public DateOnly StartDateOnly { get; set; }
        public DateOnly EndDateOnly { get; set; }
        public TimeOnly StartTimeOnly { get; set; }
        public TimeOnly EndTimeOnly { get; set; }
        public ReservationTimeRange(DateOnly startDateOnly, DateOnly endDateOnly, TimeOnly startTimeOnly, TimeOnly endTimeOnly)
        {
            Validate(startDateOnly, endDateOnly, startTimeOnly, endTimeOnly);

            StartDateOnly = startDateOnly;
            EndDateOnly = endDateOnly;
            StartTimeOnly = startTimeOnly;
            EndTimeOnly = endTimeOnly;
        }

        public ReservationTimeRange Update(DateOnly startDateOnly, DateOnly endDateOnly, TimeOnly startTimeOnly, TimeOnly endTimeOnly)
        {
            Validate(startDateOnly, endDateOnly, startTimeOnly, endTimeOnly);

            StartDateOnly = startDateOnly;
            EndDateOnly = endDateOnly;
            StartTimeOnly = startTimeOnly;
            EndTimeOnly = endTimeOnly;

            return this;
        }

        private void Validate(DateOnly startDateOnly, DateOnly endDateOnly, TimeOnly startTimeOnly, TimeOnly endTimeOnly)
        {
            if(startTimeOnly == TimeOnly.MinValue)
            {
                throw new ArgumentException("La hora de inicio no puede ser nula.", nameof(startTimeOnly));
            }
            if(endTimeOnly == TimeOnly.MinValue)
            {
                throw new ArgumentException("La hora de inicio no puede ser nula.", nameof(startTimeOnly));
            }
            if(startDateOnly == DateOnly.MinValue)
            {
                throw new ArgumentException("La fecha de inicio no puede ser nula.", nameof(startDateOnly));
            }
            if (endDateOnly == DateOnly.MinValue)
            {
                throw new ArgumentException("La fecha de inicio no puede ser nula.", nameof(startDateOnly));
            }
            if (startDateOnly >= endDateOnly)
            {
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin.", nameof(startDateOnly));
            }
            if (startTimeOnly >= endTimeOnly)
            {
                throw new ArgumentException("La hora de inicio debe ser anterior a la hora de fin.", nameof(startTimeOnly));
            }
        }
    }
}
