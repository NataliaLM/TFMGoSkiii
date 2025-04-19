using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class ClassEFConfig : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.ToTable("Class");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100); 

            builder.Property(c => c.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.StudentQuantity)
                .IsRequired();

            builder.HasOne<Instructor>()
                .WithMany()
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.ClassLevel)
                .IsRequired();
        }
    }
}
