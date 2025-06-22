using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class ReservationTimeRangeMaterialEFConfig : IEntityTypeConfiguration<ReservationTimeRangeMaterial>
    {
        public void Configure(EntityTypeBuilder<ReservationTimeRangeMaterial> builder)
        {
            builder.Property(r => r.RemainingMaterialsQuantity)
                .IsRequired();

            builder.HasOne<Material>()
                .WithMany()
                .HasForeignKey(r => r.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
