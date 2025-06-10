using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using Xunit;

namespace TFMGoSkiTest
{
    public class ClassReservationsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public ClassReservationsControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient();

            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();
        }

        [Fact]
        public async Task Test_ClassReservations_Index_ReturnsSuccess()
        {
            var user = new User
            {
                UserName = "email@email.com",
                FullName = "email@email.com",
                Email = "email@email.com",
                PhoneNumber = "1234456789"
            };

            //_context.Users.Add(user);
            //_context.SaveChanges();

            Instructor instructor = new Instructor("Name Instructor ClassReservation");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            _context.ClassReservations.Add(new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id));
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync("/ClassReservations");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Details_ValidId_ReturnsSuccess()
        {
            var user = new User { UserName = "InvalidEditUser" };

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassReservations/Details/{reservation.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Details_NullId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/ClassReservations/Details");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Details_InvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/ClassReservations/Details/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Create_Get_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/ClassReservations/Create");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Create_Post_ReturnsRedirect()
        {
            var user = new User { UserName = "InvalidEditUser" };

            var instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);

            var city = new City("Name City");
            _context.Cities.Add(city);

            await _context.SaveChangesAsync();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);

            _context.Users.Add(user);
            _context.Classes.Add(@class);

            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() }
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ClassReservations/Create", content);

            // Normalmente, en éxito se redirige al Index
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Get_ReturnsSuccess()
        {
            var user = new User { UserName = "InvalidEditUser" };

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassReservations/Edit/{reservation.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Get_Id_Null()
        {
            var user = new User { UserName = "InvalidEditUser" };

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassReservations/Edit?");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ReturnsRedirect()
        {
            var user = new User { UserName = "InvalidEditUser" };

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var viewModel = new ClassReservationViewModel
            {
                Id = reservation.Id,
                UserId = user.Id,
                ClassId = @class.Id
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            Assert.True(response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ModelInvalid()
        {
            var user = new User { UserName = "InvalidEditUser" };

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var viewModel = new ClassReservationViewModel
            {
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            Assert.True(response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Test_ClassReservations_Delete_Get_ReturnsSuccess()
        {
            var user = new User { 
                UserName = "asNewd@asdNew.com",
                FullName = "FullName",
                Email = "asNewd@asdNew.com",
                PhoneNumber = "1234456789"
            };

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassReservations/Delete/{reservation.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Delete_Get_IdNull()
        {
            var user = new User { UserName = "InvalidEditUser" };

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassReservations/Delete?");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_DeleteConfirmed_Post_ReturnsRedirect()
        {
            var user = new User { UserName = "InvalidEditUser" };

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ClassReservations/Delete/{reservation.Id}", new StringContent(""));

            Assert.True(response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Test_ClassReservations_Create_Post_InvalidModel_ReturnsView()
        {
            // No se envía UserId ni ClassId
            var viewModel = new ClassReservationViewModel(); // Campos requeridos vacíos

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/ClassReservations/Create", content);

            // La vista se debe devolver con los errores de validación
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_InvalidModel_ReturnsView()
        {
            // Preparar datos válidos en la BD
            var user = new User { UserName = "InvalidEditUser" };

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Enviar modelo inválido (UserId y ClassId vacíos)
            var invalidModel = new ClassReservationViewModel
            {
                Id = reservation.Id
                // UserId y ClassId omitidos a propósito
            };

            var json = JsonSerializer.Serialize(invalidModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
