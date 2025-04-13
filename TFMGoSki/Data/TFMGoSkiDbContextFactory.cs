using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TFMGoSki.Data
{
    public class TFMGoSkiDbContextFactory : IDesignTimeDbContextFactory<TFMGoSkiDbContext>
    {
        public TFMGoSkiDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TFMGoSkiDbContext>();

            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new TFMGoSkiDbContext(optionsBuilder.Options);
        }
    }
}
