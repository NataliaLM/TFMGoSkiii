using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class ReservationTimeRangeClassEFConfig : IEntityTypeConfiguration<ReservationTimeRangeClass>
    {
        public void Configure(EntityTypeBuilder<ReservationTimeRangeClass> builder)
        {
            builder.Property(c => c.RemainingStudentsQuantity)
                .IsRequired();

            builder.HasOne<Class>()
                .WithMany()
                .HasForeignKey(c => c.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
