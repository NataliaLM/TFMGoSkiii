namespace TFMGoSki.Models
{
    public class ReservationTimeRangeClass : ReservationTimeRange
    {
        public int RemainingStudentsQuantity { get; set; }
        public int ClassId { get; set; }

        public ReservationTimeRangeClass(DateTime startTime, DateTime endTime, int remainingStudentsQuantity, int classId)
            : base(startTime, endTime)
        {
            Validate(remainingStudentsQuantity, classId);

            RemainingStudentsQuantity = remainingStudentsQuantity;
            ClassId = classId;
        }

        public ReservationTimeRangeClass Update(DateTime startTime, DateTime endTime, int remainingStudentsQuantity, int classId)
        {
            Validate(remainingStudentsQuantity, classId);
            base.Update(startTime, endTime);

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
