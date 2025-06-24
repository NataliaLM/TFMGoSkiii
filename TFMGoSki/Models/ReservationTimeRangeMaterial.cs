namespace TFMGoSki.Models
{
    public class ReservationTimeRangeMaterial : ReservationTimeRange
    {
        public int RemainingMaterialsQuantity { get; set; }
        public int MaterialId { get; set; }

        public ReservationTimeRangeMaterial(DateOnly startDateOnly, DateOnly endDateOnly, TimeOnly startTimeOnly, TimeOnly endTimeOnly, int remainingMaterialsQuantity, int materialId)
            : base(startDateOnly, endDateOnly, startTimeOnly, endTimeOnly)
        {
            Validate(remainingMaterialsQuantity, materialId);

            RemainingMaterialsQuantity = remainingMaterialsQuantity;
            MaterialId = materialId;
        }

        public ReservationTimeRangeMaterial Update(DateOnly startDateOnly, DateOnly endDateOnly, TimeOnly startTimeOnly, TimeOnly endTimeOnly, int remainingMaterialsQuantity, int materialId)
        {
            Validate(remainingMaterialsQuantity, materialId);
            base.Update(startDateOnly, endDateOnly, startTimeOnly, endTimeOnly);

            RemainingMaterialsQuantity = remainingMaterialsQuantity;
            MaterialId = materialId;

            return this;
        }


        private void Validate(int remainingMaterialsQuantity, int materialId)
        {
            if (remainingMaterialsQuantity <= -2)
            {
                throw new ArgumentException("The amount of remaining materials must be greater than zero.", nameof(remainingMaterialsQuantity));

            }
            if (materialId <= 0)
            {
                throw new ArgumentException("The Id of the material must be greater than zero.", nameof(remainingMaterialsQuantity));
            }
        }
    }
}
