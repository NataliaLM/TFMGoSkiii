using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class MaterialReservationEFConfig : IEntityTypeConfiguration<MaterialReservation>
    {
        public void Configure(EntityTypeBuilder<MaterialReservation> builder)
        {
            builder.ToTable("MaterialReservation");

            builder.HasKey(mr => mr.Id);

            builder.Property(mr => mr.Total)
                .IsRequired();

            builder.Property(mr => mr.Paid)
                .IsRequired();

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
