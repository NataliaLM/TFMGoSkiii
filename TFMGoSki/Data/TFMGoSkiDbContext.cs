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
        /**/
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Administrator> Administrators { get; set; }
        public DbSet<Models.Worker> Workers { get; set; }
        public DbSet<Models.Client> Clients { get; set; }   
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
            modelBuilder.ApplyConfiguration(new UserEFConfig());
            modelBuilder.ApplyConfiguration(new AdministratorEFConfig());
            modelBuilder.ApplyConfiguration(new WorkerEFConfig());
            modelBuilder.ApplyConfiguration(new ClientEFConfig());
            modelBuilder.ApplyConfiguration(new ClassReservationEFConfig());
            modelBuilder.ApplyConfiguration(new ClassCommentEFConfig());
            modelBuilder.ApplyConfiguration(new CommentEFConfig());

            base.OnModelCreating(modelBuilder);
        }

    }
}
