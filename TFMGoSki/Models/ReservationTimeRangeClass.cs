namespace TFMGoSki.Models
{
    public class ReservationTimeRangeClass : ReservationTimeRange
    {
        public int RemainingStudentsQuantity { get; set; }
        public int ClassId { get; set; }

        public ReservationTimeRangeClass(DateOnly startDateOnly, DateOnly endDateOnly, TimeOnly startTimeOnly, TimeOnly endTimeOnly, int remainingStudentsQuantity, int classId)
            : base(startDateOnly, endDateOnly, startTimeOnly, endTimeOnly)
        {
            Validate(remainingStudentsQuantity, classId);

            RemainingStudentsQuantity = remainingStudentsQuantity;
            ClassId = classId;
        }

        public ReservationTimeRangeClass Update(DateOnly startDateOnly, DateOnly endDateOnly, TimeOnly startTimeOnly, TimeOnly endTimeOnly, int remainingStudentsQuantity, int classId)
        {
            Validate(remainingStudentsQuantity, classId);
            base.Update(startDateOnly, endDateOnly, startTimeOnly, endTimeOnly);

            RemainingStudentsQuantity = remainingStudentsQuantity;
            ClassId = classId;

            return this;
        }


        private void Validate(int remainingStudentsQuantity, int classId)
        {
            if (remainingStudentsQuantity <= 0)
            {
                throw new ArgumentException("La cantidad de estudiantes restante debe ser mayor que cero.", nameof(remainingStudentsQuantity));

            }
            if (classId <= 0)
            {
                throw new ArgumentException("El Id de la clase debe ser mayor que cero.", nameof(remainingStudentsQuantity));
            }
        }
    }
}
