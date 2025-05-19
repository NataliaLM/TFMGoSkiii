using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

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

            base.OnModelCreating(modelBuilder);
        }        
        public DbSet<TFMGoSki.ViewModels.RegisterViewModel> RegisterViewModel { get; set; } = default!;
        public DbSet<TFMGoSki.ViewModels.LoginViewModel> LoginViewModel { get; set; } = default!;
    }
}
