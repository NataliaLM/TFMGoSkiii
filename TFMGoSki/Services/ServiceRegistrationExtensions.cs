using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;

namespace TFMGoSki.Services
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IHostEnvironment env)
        {
            if (env.IsEnvironment("Test"))
            {
                services.AddDbContext<TFMGoSkiDbContext>(options =>
                    options.UseInMemoryDatabase("InMemoryDb"));
            }
            else
            {
                services.AddDbContext<TFMGoSkiDbContext>(options =>
                    options.UseSqlServer("name=DefaultConnection"));
            }

            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IInstructorService, InstructorService>();
            services.AddScoped<IReservationTimeRangeClassService, ReservationTimeRangeClassService>();

            return services;
        }
    }
}
