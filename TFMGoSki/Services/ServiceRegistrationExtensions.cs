using Microsoft.Extensions.DependencyInjection;
using TFMGoSki.Services;

namespace TFMGoSki.Services
{
    public static class ServiceRegistrationExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IInstructorService, InstructorService>();
            services.AddScoped<IReservationTimeRangeClassService, ReservationTimeRangeClassService>();

        }
    }
}
