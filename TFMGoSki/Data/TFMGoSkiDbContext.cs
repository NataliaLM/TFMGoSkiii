using Microsoft.EntityFrameworkCore;

namespace TFMGoSki.Data
{
    public class TFMGoSkiDbContext : DbContext
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClassEFConfig());
            modelBuilder.ApplyConfiguration(new InstructorEFConfig());
            modelBuilder.ApplyConfiguration(new ReservationTimeRangeEFConfig());
            modelBuilder.ApplyConfiguration(new ReservationTimeRangeClassEFConfig());
            modelBuilder.ApplyConfiguration(new CityEFConfig());

            base.OnModelCreating(modelBuilder);
        }

    }
}
