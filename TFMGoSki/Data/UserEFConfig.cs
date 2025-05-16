using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class UserEFConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Phone)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Password)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne<Role>()
                .WithMany()
                .HasForeignKey(c => c.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
