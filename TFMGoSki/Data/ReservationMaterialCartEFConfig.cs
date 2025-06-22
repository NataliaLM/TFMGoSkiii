using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class ReservationMaterialCartEFConfig : IEntityTypeConfiguration<ReservationMaterialCart>
    {
        public void Configure(EntityTypeBuilder<ReservationMaterialCart> builder)
        {
            builder.ToTable("ReservationMaterialCart");

            builder.HasKey(rmc => rmc.Id);

            builder.HasOne<Material>()
                .WithMany()
                .HasForeignKey(rmc => rmc.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<MaterialReservation>()
                .WithMany()
                .HasForeignKey(rmc => rmc.MaterialReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ReservationTimeRangeMaterial>()
               .WithMany()
               .HasForeignKey(c => c.ReservationTimeRangeMaterialId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.NumberMaterialsBooked)
                .IsRequired();
        }
    }
}
