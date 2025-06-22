using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class MaterialStatusEFConfig : IEntityTypeConfiguration<MaterialStatus>
    {
        public void Configure(EntityTypeBuilder<MaterialStatus> builder)
        {
            builder.ToTable("MaterialStatus");

            builder.HasKey(ms => ms.Id);

            builder.Property(ms => ms.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
