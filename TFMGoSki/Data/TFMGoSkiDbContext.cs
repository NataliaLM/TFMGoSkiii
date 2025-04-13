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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClassEFConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}
