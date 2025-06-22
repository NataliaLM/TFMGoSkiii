using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class MaterialCommentEFConfig : IEntityTypeConfiguration<MaterialComment>
    {
        public void Configure(EntityTypeBuilder<MaterialComment> builder)
        {

            builder.HasOne<ReservationMaterialCart>()
                .WithMany()
                .HasForeignKey(mc => mc.ReservationMaterialCartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
