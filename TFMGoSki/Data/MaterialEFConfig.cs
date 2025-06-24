using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class MaterialEFConfig : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.ToTable("Material");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(m => m.QuantityMaterial)
                .IsRequired();

            builder.Property(m => m.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(m => m.Size)
                .IsRequired();

            builder.HasOne<City>()
                .WithMany()
                .HasForeignKey(m => m.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<MaterialType>()
                .WithMany()
                .HasForeignKey(m => m.MaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<MaterialStatus>()
                .WithMany()
                .HasForeignKey(m => m.MaterialStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
