﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFMGoSki.Models;

namespace TFMGoSki.Data
{
    public class ClassReservationEFConfig : IEntityTypeConfiguration<ClassReservation>
    {
        public void Configure(EntityTypeBuilder<ClassReservation> builder)
        {
            builder.ToTable("ClassReservation");

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Class>()
               .WithMany()
               .HasForeignKey(c => c.ClassId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
