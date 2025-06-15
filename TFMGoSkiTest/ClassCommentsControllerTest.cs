using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using Xunit;

namespace TFMGoSkiTest
{
    public class ClassCommentsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public ClassCommentsControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Test"));
            _client = _factory.CreateClient();

            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();
        }

        [Fact]
        public async Task Index_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            _context.ClassComments.Add(new ClassComment(classReservation.Id, "Great class", 5));
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync("/ClassComments");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Details_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassComments/Details/{classComment.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Details_IdNull_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/ClassComments/Details");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync("/ClassComments/Create");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_Valid_ReturnsRedirect()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "classReservationId", "1" },
                { "Text", "Great lesson!" },
                { "Raiting", "5" }
            };
            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/ClassComments/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_Invalid()
        {
            var formData = new Dictionary<string, string>
            {
            };
            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/ClassComments/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassComments/Edit/{classComment.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Get_NotFound()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassComments/Edit?");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_Valid_UpdatesEntry()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Original comment", 3);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", classComment.Id.ToString() },
                { "ClassReservationId", classReservation.Id.ToString() },
                { "Text", "Updated comment" },
                { "Raiting", "4" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassComments/Edit/{classComment.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassComments/Delete/{classComment.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Get_Invalid()
        {
            var response = await _client.GetAsync($"/ClassComments/Delete?");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Post_RemovesEntry()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ClassComments/Delete/{classComment.Id}", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
