using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using TFMGoSki.Dtos;

namespace TFMGoSki.Data
{
    public class TFMGoSkiDbContext : IdentityDbContext<User, Role, int>
    {
        public TFMGoSkiDbContext(DbContextOptions<TFMGoSkiDbContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Class> Classes { get; set; }
        public DbSet<Models.Instructor> Instructors { get; set; }
        public DbSet<Models.ReservationTimeRange> ReservationTimeRanges { get; set; }
        public DbSet<Models.ReservationTimeRangeClass> ReservationTimeRangeClasses { get; set; }
        public DbSet<Models.City> Cities { get; set; }
        /**/
        public DbSet<Models.ClassReservation> ClassReservations { get; set; }
        public DbSet<Models.ClassComment> ClassComments { get; set; }
        public DbSet<Models.Comment> Comments { get; set; }
        /**/
        public DbSet<Models.MaterialComment> MaterialComments { get; set; }
        public DbSet<Models.Material> Materials { get; set; }
        public DbSet<Models.MaterialReservation> MaterialReservations { get; set; }
        public DbSet<Models.MaterialStatus> MaterialStatuses { get; set; }
        public DbSet<Models.MaterialType> MaterialTypes { get; set; }
        public DbSet<Models.ReservationMaterialCart> ReservationMaterialCarts { get; set; }
        public DbSet<Models.ReservationTimeRangeMaterial> ReservationTimeRangeMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClassEFConfig());
            modelBuilder.ApplyConfiguration(new InstructorEFConfig());
            modelBuilder.ApplyConfiguration(new ReservationTimeRangeEFConfig());
            modelBuilder.ApplyConfiguration(new ReservationTimeRangeClassEFConfig());
            modelBuilder.ApplyConfiguration(new CityEFConfig());
            /**/
            modelBuilder.ApplyConfiguration(new ClassReservationEFConfig());
            modelBuilder.ApplyConfiguration(new ClassCommentEFConfig());
            modelBuilder.ApplyConfiguration(new CommentEFConfig());
            /**/
            modelBuilder.ApplyConfiguration(new MaterialCommentEFConfig());
            modelBuilder.ApplyConfiguration(new MaterialEFConfig());
            modelBuilder.ApplyConfiguration(new MaterialReservationEFConfig());
            modelBuilder.ApplyConfiguration(new MaterialStatusEFConfig());
            modelBuilder.ApplyConfiguration(new MaterialTypeEFConfig());
            modelBuilder.ApplyConfiguration(new ReservationMaterialCartEFConfig());
            modelBuilder.ApplyConfiguration(new ReservationTimeRangeMaterialEFConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}
