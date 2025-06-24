using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class MaterialTypeEFConfig : IEntityTypeConfiguration<MaterialType>
    {
        public void Configure(EntityTypeBuilder<MaterialType> builder)
        {
            builder.ToTable("MaterialType");

            builder.HasKey(mt => mt.Id);

            builder.Property(mt => mt.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
