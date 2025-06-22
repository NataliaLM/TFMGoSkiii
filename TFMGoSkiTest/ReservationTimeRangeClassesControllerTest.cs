using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class ReservationTimeRangeClassesControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public ReservationTimeRangeClassesControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient();

            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();

            //Llamada opcional si quieres que se loguee automáticamente
            Task.Run(() => AuthenticateAsync()).Wait();
        }

        private async Task AuthenticateAsync(string role = "Admin")
        {
            //var responseLogout = _client.PostAsync("/Account/Logout", new StringContent(""));

            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities",
                PhoneNumber = "223456789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            var loginData = new Dictionary<string, string>
            {
                { "UserName", testEmail },
                { "Password", testPassword }
            };

            var loginContent = new FormUrlEncodedContent(loginData);
            var response = await _client.PostAsync("/Account/Login", loginContent);
            response.EnsureSuccessStatusCode();
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2025, 06, 21), 
                new DateOnly(2025, 07, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 25, @class.Id);
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
        public async Task Test_Reservation_Details_ReturnsNotFound_IdNull()
        {
            var response = await _client.GetAsync("/ReservationTimeRangeClasses/Details?");
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

            var formData = new Dictionary<string, string>
            {
                ["Class"] = @class.Id.ToString(),
                ["StartDateOnly"] = "2025-08-21",
                ["EndDateOnly"] = "2025-09-22",
                ["StartTimeOnly"] = "11:25:46",
                ["EndTimeOnly"] = "12:26:47"
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Post_RedirectsToIndex_DateBefore()
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

            var formData = new Dictionary<string, string>
            {
                ["Class"] = @class.Id.ToString(),
                ["StartDateOnly"] = "2025-09-22",
                ["EndDateOnly"] = "2025-08-21",
                ["StartTimeOnly"] = "11:25:46",
                ["EndTimeOnly"] = "12:26:47"
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Post_RedirectsToIndex_InvalidModelHoure()
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

            var formData = new Dictionary<string, string>
            {
                ["Class"] = @class.Id.ToString(),
                ["StartDateOnly"] = "2025-08-21",
                ["EndDateOnly"] = "2025-08-21",
                ["StartTimeOnly"] = "11:26:47",
                ["EndTimeOnly"] = "12:25:46"
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Post_RedirectsToIndex_DateBeforeNow()
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

            var formData = new Dictionary<string, string>
            {
                ["Class"] = @class.Id.ToString(),
                ["StartDateOnly"] = "2024-08-21",
                ["EndDateOnly"] = "2024-10-21",
                ["StartTimeOnly"] = "11:26:47",
                ["EndTimeOnly"] = "12:25:46"
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Post_RedirectsToIndex_MinValues()
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

            var formData = new Dictionary<string, string>
            {
                ["Class"] = @class.Id.ToString(),
                ["StartDateOnly"] = DateOnly.MinValue.ToString(),
                ["EndDateOnly"] = DateOnly.MinValue.ToString(),
                ["StartTimeOnly"] = TimeOnly.MinValue.ToString(),
                ["EndTimeOnly"] = TimeOnly.MinValue.ToString()
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Post_InvalidModel()
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2025, 10, 21), new DateOnly(2025, 11, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Edit/{reservationTimeRangeClass.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Get_NotFound_Zero()
        {
            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Edit?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Get_NotFound_Id()
        {
            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Edit/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2025, 12, 21), new DateOnly(2026, 01, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                ["Class"] = @class.Id.ToString(),
                ["StartDateOnly"] = "2025-08-21",
                ["EndDateOnly"] = "2025-09-22",
                ["StartTimeOnly"] = "11:25:46",
                ["EndTimeOnly"] = "12:26:47"
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Edit/{reservationTimeRangeClass.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Post_InvalidModel()
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2026, 04, 21), new DateOnly(2026, 05, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var viewModel = new ReservationTimeRangeClassViewModel
            {
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Edit/{reservationTimeRangeClass.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Post_NotFound_Id()
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(
                new DateOnly(2026, 06, 21),
                new DateOnly(2026, 07, 22),
                new TimeOnly(11, 25, 46),
                new TimeOnly(12, 26, 47),
                15,
                @class.Id
            );

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Class", @class.Id.ToString() },
                { "StartDateOnly", "2026-08-21" },
                { "EndDateOnly", "2026-09-22" },
                { "StartTimeOnly", "11:25:46" },
                { "EndTimeOnly", "12:26:47" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Edit/999", content);

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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2026, 10, 21), new DateOnly(2026, 11, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Delete/{reservationTimeRangeClass.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Get_NotFound_Zero()
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2026, 12, 21), new DateOnly(2027, 01, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Delete?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Get_NotFound()
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2027, 02, 21), new DateOnly(2027, 03, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Delete/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2027, 04, 21), new DateOnly(2027, 05, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Delete/{reservationTimeRangeClass.Id}", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Test_Reservation_Delete_Post_Redirects_TieneClassReservation()
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2027, 04, 21), new DateOnly(2027, 05, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            #region user
            string role = "Admin";

            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades123123.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities asd",
                PhoneNumber = "223324389"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);
            #endregion

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 12);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Delete/{reservationTimeRangeClass.Id}", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Post_NotFound()
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

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2027, 06, 21), new DateOnly(2027, 07, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Delete/999", new StringContent(""));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        /**/
        [Fact]
        public async Task Test_Reservation_Create_Get_WithClassId_ReturnsSuccess()
        {
            var classId = 1; // Asegúrate de que este ID exista o lo creas antes
            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Create?classId={classId}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Test_Reservation_Create_Post_InvalidModel_WithClass()
        {
            var viewModel = new ReservationTimeRangeClassViewModel
            {
                Class = 123 // Valor cualquiera distinto de 0
                            // Campos faltantes, forzando ModelState inválido
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Create", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //[Fact]
        //public async Task Test_Reservation_Delete_Post_WithAssociatedReservations_ShowsError()
        //{
        //    Instructor instructor = new Instructor("Name Instructor");
        //    _context.Instructors.Add(instructor);
        //    _context.SaveChanges();

        //    City city = new City("City Name");
        //    _context.Cities.Add(city);
        //    _context.SaveChanges();

        //    Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
        //    _context.Classes.Add(@class);
        //    _context.SaveChanges();

        //    var timeRange = new ReservationTimeRangeClass(new DateOnly(2027, 01, 01), new DateOnly(2027, 01, 10),
        //        new TimeOnly(9, 0), new TimeOnly(10, 0), 10, @class.Id);
        //    _context.ReservationTimeRangeClasses.Add(timeRange);
        //    _context.SaveChanges();

        //    var reservation = new ClassReservation(user.Id, @class.Id, timeRange.Id, 1);
            
        //    _context.ClassReservations.Add(reservation);
        //    _context.SaveChanges();

        //    var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Delete/{timeRange.Id}", new StringContent(""));

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    var content = await response.Content.ReadAsStringAsync();
        //    Assert.Contains("cannot be deleted", content); // o lo que diga tu error exacto
        //}
    }
}
