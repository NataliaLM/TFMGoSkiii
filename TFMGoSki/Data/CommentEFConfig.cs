using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class CommentEFConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comment");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Text)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Raiting)
                .IsRequired();

            builder.HasDiscriminator<string>("Discriminator")
                .HasValue<ClassComment>("ClassComment");
        }
    }
}
