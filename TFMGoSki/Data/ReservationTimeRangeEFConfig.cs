using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class ReservationTimeRangeEFConfig : IEntityTypeConfiguration<ReservationTimeRange>
    {
        public void Configure(EntityTypeBuilder<ReservationTimeRange> builder)
        {
            builder.ToTable("ReservationTimeRange");

            builder.HasKey(c => c.Id);

            builder.HasDiscriminator<string>("Discriminator")
                .HasValue<ReservationTimeRangeClass>("ReservationTimeRangeClass");

            builder.Property(c => c.StartTime)
                .IsRequired();

            builder.Property(c => c.EndTime)
                .IsRequired();
        }
    }
}
