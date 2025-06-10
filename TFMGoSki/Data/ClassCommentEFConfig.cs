using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class ClassCommentEFConfig : IEntityTypeConfiguration<ClassComment>
    {
        public void Configure(EntityTypeBuilder<ClassComment> builder)
        {
            builder.HasOne<ClassReservation>()
                .WithMany()
                .HasForeignKey(c => c.ClassReservationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
