﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Net;
using System.Text;
using System.Text.Json;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class ClassReservationsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;
        private readonly UserManager<User> _userManager;

        public ClassReservationsControllerTest(WebApplicationFactory<Program> factory)
        {            
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient();

            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();  // <-- Fix here


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

        private async Task<string> AuthenticateClientAsync(string role = "Client")
        {
            //var responseLogout = _client.PostAsync("/Account/Logout", new StringContent(""));

            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Client@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Client",
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

            return testEmail;
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

            _context.ClassReservations.Add(new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1));
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync("/ClassReservations");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_IndexUser_ReturnsSuccess()
        {
            //var user = new User
            //{
            //    UserName = "email123@email123.com",
            //    FullName = "email123@email.com",
            //    Email = "email123@email.com",
            //    PhoneNumber = "3214456789"
            //};

            //_context.Users.Add(user);
            //_context.SaveChanges();

            Instructor instructor = new Instructor("Name Instructor ClassReservation");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            //_context.Users.Add(user);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            #region clienteUser

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            string correo = await AuthenticateClientAsync();

            var user = await _userManager.FindByEmailAsync(correo);
            var userId = user.Id;

            #endregion

            _context.ClassReservations.Add(new ClassReservation(userId, @class.Id, reservationTimeRangeClass.Id, 1));
            await _context.SaveChangesAsync();

                    
            var response = await _client.GetAsync("/ClassReservations/IndexUser");

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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassReservations/Details/{reservation.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Details_ValidId_ReturnsSuccess_Client()
        {
            #region clienteUser

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            string correo = await AuthenticateClientAsync();

            #endregion

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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
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
            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            Task.Run(() => AuthenticateClientAsync()).Wait();

            var response = await _client.GetAsync("/ClassReservations/Create");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Create_Post_ReturnsRedirect()
        {
            var role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities123@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities A",
                PhoneNumber = "243456789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            var instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);

            var city = new City("Name City");
            _context.Cities.Add(city);

            await _context.SaveChangesAsync();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);

            
            _context.Classes.Add(@class);

            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "12" }
            };

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            Task.Run(() => AuthenticateClientAsync()).Wait();

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ClassReservations/Create", content);

            // Normalmente, en éxito se redirige al Index
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Create_Post_ReturnsRedirect_RemainingStudentQuantityZero()
        {
            var role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities123@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities A",
                PhoneNumber = "243456789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            var instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);

            var city = new City("Name City");
            _context.Cities.Add(city);

            await _context.SaveChangesAsync();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);


            _context.Classes.Add(@class);

            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 0, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "12" }
            };

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            Task.Run(() => AuthenticateClientAsync()).Wait();

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ClassReservations/Create", content);

            // Normalmente, en éxito se redirige al Index
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Test_ClassReservations_Create_Post_ReturnsRedirect_NoQuedanPlazas()
        {
            var role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities123@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities A",
                PhoneNumber = "243456789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            var instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);

            var city = new City("Name City");
            _context.Cities.Add(city);

            await _context.SaveChangesAsync();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);


            _context.Classes.Add(@class);

            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), -1, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "12" }
            };

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            Task.Run(() => AuthenticateClientAsync()).Wait();

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ClassReservations/Create", content);

            // Normalmente, en éxito se redirige al Index
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Test_ClassReservations_Create_Post_ReturnsRedirect_SeQuedaSinPlazas()
        {
            var role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities123@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities A",
                PhoneNumber = "243456789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            var instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);

            var city = new City("Name City");
            _context.Cities.Add(city);

            await _context.SaveChangesAsync();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);


            _context.Classes.Add(@class);

            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 0, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "2" }
            };

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            Task.Run(() => AuthenticateClientAsync()).Wait();

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ClassReservations/Create", content);

            // Normalmente, en éxito se redirige al Index
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Test_ClassReservations_Create_Post_ReturnsRedirect_InvalidModel()
        {
            var role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities123@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities A",
                PhoneNumber = "243456789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            var instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);

            var city = new City("Name City");
            _context.Cities.Add(city);

            await _context.SaveChangesAsync();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);


            _context.Classes.Add(@class);

            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
            };

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            Task.Run(() => AuthenticateClientAsync()).Wait();

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ClassReservations/Create", content);

            // Normalmente, en éxito se redirige al Index
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Test_ClassReservations_Create_Post_ReturnsRedirect_ClienteUser()
        {
            var role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities123@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities A",
                PhoneNumber = "243456789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            var instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);

            var city = new City("Name City");
            _context.Cities.Add(city);

            await _context.SaveChangesAsync();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);


            _context.Classes.Add(@class);

            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "1" }
            };

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            Task.Run(() => AuthenticateClientAsync()).Wait();

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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassReservations/Edit/{reservation.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Get_ReturnsSuccess_Client()
        {
            #region clienteUser

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            string correo = await AuthenticateClientAsync();

            var userLogin = await _userManager.FindByEmailAsync(correo);

            #endregion

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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClassToUpdate = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(3)), DateOnly.FromDateTime(DateTime.Today.AddDays(4)), TimeOnly.FromDateTime(DateTime.Now.AddHours(3)), TimeOnly.FromDateTime(DateTime.Now.AddHours(4)), 9, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClassToUpdate);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClassToUpdate.Id.ToString() },
                { "NumberPersonsBooked", "1" } // Asegúrate de enviar todos los campos requeridos
            };

            var content = new FormUrlEncodedContent(formData);

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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                ["Id"] = reservation.Id.ToString()
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ReturnsRedirect_IsInRolClient()
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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClassToUpdate = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(3)), DateOnly.FromDateTime(DateTime.Today.AddDays(4)), TimeOnly.FromDateTime(DateTime.Now.AddHours(3)), TimeOnly.FromDateTime(DateTime.Now.AddHours(4)), 9, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClassToUpdate);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClassToUpdate.Id.ToString() },
                { "NumberPersonsBooked", "2" } // Asegúrate de enviar todos los campos requeridos
            };

            var content = new FormUrlEncodedContent(formData);

            #region clienteUser

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            string correo = await AuthenticateClientAsync();

            var userLogin = await _userManager.FindByEmailAsync(correo);

            #endregion

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            Assert.True(response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ReturnsRedirect_NumberBooked()
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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "2" } 
            };

            var content = new FormUrlEncodedContent(formData);

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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(reservation.Id, "text", 4);
            _context.Add(classComment);
            _context.SaveChanges();

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
        public async Task Test_ClassReservations_Create_Post_ValidModel_ReturnsView()
        {
            var role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities123@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities A",
                PhoneNumber = "243456789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            var instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var city = new City("Name City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                ["UserId"] = user.Id.ToString(),
                ["ClassId"] = @class.Id.ToString(),
                ["ReservationTimeRangeClassId"] = reservationTimeRangeClass.Id.ToString(),
                ["NumberPersonsBooked"] = "1"
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/ClassReservations/Create", content);

            // La vista se debe devolver con los errores de validación
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Create_Post_ValidModel_ReturnsView_ZeroRemainingStudents()
        {
            var role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities123@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities A",
                PhoneNumber = "243456789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            var instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var city = new City("Name City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 0, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                ["UserId"] = user.Id.ToString(),
                ["ClassId"] = @class.Id.ToString(),
                ["ReservationTimeRangeClassId"] = reservationTimeRangeClass.Id.ToString(),
                ["NumberPersonsBooked"] = "1"
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/ClassReservations/Create", content);

            // La vista se debe devolver con los errores de validación
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ValidModel_ReturnsView()
        {
            string role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades321.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities B",
                PhoneNumber = "223216789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "1" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
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

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        
        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ValidModel_ReturnsView_ClientLoged()
        {
            #region clienteUser

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            string correo = await AuthenticateClientAsync();

            var userLogin = await _userManager.FindByEmailAsync(correo);

            #endregion

            string role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades321.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities B",
                PhoneNumber = "223216789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "1" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ValidModel_ReturnsView_RemainingStudentsQuantityMinusOne()
        {
            #region clienteUser

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            string correo = await AuthenticateClientAsync();

            var userLogin = await _userManager.FindByEmailAsync(correo);

            #endregion

            string role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades321.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities B",
                PhoneNumber = "223216789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), -1, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "1" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ValidModel_ReturnsView_ClientLoged_UpdateReservationTimeRangeClassId()
        {
            #region clienteUser

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            string correo = await AuthenticateClientAsync();

            var userLogin = await _userManager.FindByEmailAsync(correo);

            #endregion

            string role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades321.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities B",
                PhoneNumber = "223216789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClassUpdate = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 7, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClassUpdate);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClassUpdate.Id.ToString() },
                { "NumberPersonsBooked", "1" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ValidModel_ReturnsView_ClientLoged_UpdateReservationTimeRangeClassId_MinusOne()
        {
            #region clienteUser

            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            string correo = await AuthenticateClientAsync();

            var userLogin = await _userManager.FindByEmailAsync(correo);

            #endregion

            string role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades321.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities B",
                PhoneNumber = "223216789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClassUpdate = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), -1, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClassUpdate);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClassUpdate.Id.ToString() },
                { "NumberPersonsBooked", "1" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ValidModel_ReturnsView_InvalidId()
        {
            string role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades321.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities B",
                PhoneNumber = "223216789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", "123" },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClass.Id.ToString() },
                { "NumberPersonsBooked", "1" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ValidModel_ReturnsView_RemainingStudentQuantityZero()
        {
            string role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades321.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities B",
                PhoneNumber = "223216789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClassUpdated = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 0, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClassUpdated);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClassUpdated.Id.ToString() },
                { "NumberPersonsBooked", "1" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_ClassReservations_Edit_Post_ValidModel_ReturnsView_NotEnoughPlaces()
        {
            string role = "Admin";
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Cities@exampleCiudades321.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Cities B",
                PhoneNumber = "223216789"
            };

            await userManager.CreateAsync(user, testPassword);
            await userManager.AddToRoleAsync(user, role);

            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("Name City");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("InvalidEditClass", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 9, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var reservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(reservation);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClassUpdated = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 4, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClassUpdated);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "ClassId", @class.Id.ToString() },
                { "ReservationTimeRangeClassId", reservationTimeRangeClassUpdated.Id.ToString() },
                { "NumberPersonsBooked", "8" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassReservations/Edit/{reservation.Id}", content);

            // Esperamos que la vista se devuelva con errores, no redirección
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        /**/

        [Fact]
        public async Task Create_ReturnsOK_WhenClientIsNull()
        {
            // Arrange
            await AuthenticateClientAsync();

            var invalidUserId = 9999; // un ID que no exista
            var classId = 1;
            var reservationId = 1;

            // Act
            var response = await _client.GetAsync($"/ClassReservations/Create?classId={classId}&reservationTimeRangeClassId={reservationId}");

            // El controller internamente buscaría al usuario y no lo encontraría.

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTimeRangesByClassId_ReturnsCorrectTimeRanges()
        {
            Instructor instructor = new Instructor("instructor");
            City city = new City("city");
            _context.Add(instructor);
            _context.Add(city);
            _context.SaveChanges();
            // Arrange
            var classEntity = new Class("Ski Class", 12.12m, 12,ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(classEntity);
            await _context.SaveChangesAsync();

            var timeRange1 = new ReservationTimeRangeClass(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TimeOnly.FromTimeSpan(new TimeSpan(10, 0, 0)),
                TimeOnly.FromTimeSpan(new TimeSpan(12, 0, 0)),
                5,
                classEntity.Id);

            var timeRange2 = new ReservationTimeRangeClass(
                DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                TimeOnly.FromTimeSpan(new TimeSpan(14, 0, 0)),
                TimeOnly.FromTimeSpan(new TimeSpan(16, 0, 0)),
                3,
                classEntity.Id);

            _context.ReservationTimeRangeClasses.AddRange(timeRange1, timeRange2);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/ClassReservations/GetTimeRangesByClassId?classId={classEntity.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
             
            Assert.Contains($"{timeRange1.Id}", responseString);
            Assert.Contains($"{timeRange2.Id}", responseString);
        }


    }
}
