using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSkiTest
{
    public class ReservationTimeRangeClassesControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TFMGoSkiDbContext _context;

        public ReservationTimeRangeClassesControllerTest(WebApplicationFactory<Program> factory)
        {
            var webApp = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });

            _client = webApp.CreateClient();

            var scope = webApp.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();
        }

        [Fact]
        public async Task Test_Reservation_Index_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/ReservationTimeRangeClasses");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Details()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("Name class", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2015, 10, 21), new DateOnly(2016, 11, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 25, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            _context.SaveChanges();
            
            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Details/{reservationTimeRangeClass.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Details_ReturnsNotFound_ForInvalidId()
        {
            var response = await _client.GetAsync("/ReservationTimeRangeClasses/Details/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Get_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/ReservationTimeRangeClasses/Create");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Post_RedirectsToIndex()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            var viewModel = new ReservationTimeRangeClassViewModel
            {
                Class = @class.Id,
                StartDateOnly = new DateOnly(2015, 10, 21),
                EndDateOnly = new DateOnly(2016, 11, 22),
                StartTimeOnly = new TimeOnly(11, 25, 46),
                EndTimeOnly = new TimeOnly(12, 26, 47)
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2015, 10, 21), new DateOnly(2016, 11, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Edit/{reservationTimeRangeClass.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Post_Redirects()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2015, 10, 21), new DateOnly(2016, 11, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var viewModel = new ReservationTimeRangeClassViewModel
            {
                Class = @class.Id,
                StartDateOnly = new DateOnly(2015, 10, 21),
                EndDateOnly = new DateOnly(2016, 11, 22),
                StartTimeOnly = new TimeOnly(11, 25, 46),
                EndTimeOnly = new TimeOnly(12, 26, 47)
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Edit/{reservationTimeRangeClass.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2015, 10, 21), new DateOnly(2016, 11, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Delete/{reservationTimeRangeClass.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Post_Redirects()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2015, 10, 21), new DateOnly(2016, 11, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Delete/{reservationTimeRangeClass.Id}", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
