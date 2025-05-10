using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class ClassReservationEFConfig : IEntityTypeConfiguration<ClassReservation>
    {
        public void Configure(EntityTypeBuilder<ClassReservation> builder)
        {
            builder.ToTable("ClassReservation");

            builder.HasOne<Client>()
               .WithMany()
               .HasForeignKey(c => c.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Class>()
               .WithMany()
               .HasForeignKey(c => c.ClassId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
