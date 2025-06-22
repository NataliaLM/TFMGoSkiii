using System;

namespace TFMGoSki.Models
{
    public class ReservationMaterialCart
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int MaterialReservationId { get; set; }
        public int UserId { get; set; }
        public int ReservationTimeRangeMaterialId { get; set; }
        public int NumberMaterialsBooked { get; set; }

        public ReservationMaterialCart(int materialId, int materialReservationId, int userId, int reservationTimeRangeMaterialId, int numberMaterialsBooked)
        {
            Validate(materialId, materialReservationId, userId, reservationTimeRangeMaterialId, numberMaterialsBooked);

            MaterialId = materialId;
            MaterialReservationId = materialReservationId;
            UserId = userId;
            ReservationTimeRangeMaterialId = reservationTimeRangeMaterialId;
            NumberMaterialsBooked = numberMaterialsBooked;
        }

        public ReservationMaterialCart Update(int materialId, int materialReservationId, int userId, int reservationTimeRangeMaterialId, int numberMaterialsBooked)
        {
            Validate(materialId, materialReservationId, userId, reservationTimeRangeMaterialId, numberMaterialsBooked);
            MaterialId = materialId;
            MaterialReservationId = materialReservationId;
            UserId = userId;
            ReservationTimeRangeMaterialId = reservationTimeRangeMaterialId;
            NumberMaterialsBooked = numberMaterialsBooked;
            return this;
        }

        private void Validate(int materialId, int materialReservationId, int userId, int reservationTimeRangeMaterialId, int numberMaterialsBooked)
        {
            if (materialId <= 0)
            {
                throw new ArgumentException("The materialId cannot be less than zero.", nameof(materialId));
            }
            if (materialReservationId <= 0)
            {
                throw new ArgumentException("The materialReservationId cannot be less than zero.", nameof(materialReservationId));
            }
            if (userId <= 0)
            {
                throw new ArgumentException("The userId cannot be less than zero.", nameof(userId));
            }
            if (reservationTimeRangeMaterialId <= 0)
            {
                throw new ArgumentException("The reservationTimeRangeMaterialId cannot be less than zero.", nameof(reservationTimeRangeMaterialId));
            }
            if (numberMaterialsBooked <= 0)
            {
                throw new ArgumentException("The numberMaterialsBooked cannot be less than zero.", nameof(numberMaterialsBooked));
            }
        }
    }
}
