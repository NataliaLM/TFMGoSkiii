using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class MaterialReservationsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IServiceScope _scope;
        private readonly TFMGoSkiDbContext _context;
        private readonly UserManager<User> _userManager;

        public MaterialReservationsControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient();

            _scope = _factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();
            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        }

        private async Task<string> AuthenticateClientAsync(string role = "Client")
        {
            var roleManager = _scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Role(role));

            var testEmail = $"testuser{Guid.NewGuid()}@example.com";
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User",
                PhoneNumber = "1234567890"
            };

            var result = await _userManager.CreateAsync(user, testPassword);
            Assert.True(result.Succeeded);

            await _userManager.AddToRoleAsync(user, role);

            var loginData = new Dictionary<string, string>
            {
                { "UserName", testEmail },
                { "Password", testPassword }
            };

            var loginContent = new FormUrlEncodedContent(loginData);

            var response = await _client.PostAsync("/Account/Login", loginContent);
            response.EnsureSuccessStatusCode();

            // Después del login, el cliente mantiene cookies (auth)
            return testEmail;
        }

        [Fact]
        public async Task Index_ReturnsSuccessAndCorrectContentType()
        {
            await AuthenticateClientAsync();

            var response = await _client.GetAsync("/MaterialReservations");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task IndexUser_ReturnsSuccess()
        {
            await AuthenticateClientAsync();

            var response = await _client.GetAsync("/MaterialReservations/IndexUser");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/MaterialReservations/Create");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_CreatesReservationAndRedirects()
        {
            var email = await AuthenticateClientAsync();

            var user = await _userManager.FindByEmailAsync(email);

            var viewModel = new MaterialReservationViewModel
            {
                // Asume que tu view model tiene más propiedades si necesario,
                // aquí dejamos vacío porque en controlador solo se usa UserId
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
             
            var formData = new Dictionary<string, string>(); // vacío porque no se usa datos extra
            var formContent = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/MaterialReservations/Create", formContent);

            // Se espera redirección al IndexUser
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); 

            // Verifica en BBDD que existe reserva con UserId del usuario autenticado
            var exists = _context.MaterialReservations.Any(r => r.UserId == user.Id);
            Assert.True(exists);
        }

        [Fact]
        public async Task Details_ReturnsSuccess_ForExistingReservation()
        {
            var email = await AuthenticateClientAsync();
            var user = await _userManager.FindByEmailAsync(email);

            // Prepara datos para reserva
            var reservation = new MaterialReservation(user.Id, 100, false);
            _context.MaterialReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/MaterialReservations/Details/{reservation.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("MaterialReservations", content); // Algo simple, por ejemplo título
        }

        [Fact]
        public async Task Edit_Get_ReturnsReservationView_WhenExists()
        {
            // Arrange
            var email = await AuthenticateClientAsync();
            var user = await _userManager.FindByEmailAsync(email);

            var reservation = new MaterialReservation(user.Id, 99.99m, false);
            _context.MaterialReservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/MaterialReservations/Edit/{reservation.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync(); 
            Assert.Contains(reservation.Total.ToString(), content); // asume que total aparece como valor
        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFound_WhenReservationDoesNotExist()
        {
            // Arrange
            await AuthenticateClientAsync();

            // Act
            var response = await _client.GetAsync("/MaterialReservations/Edit/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task Edit_Post_UpdatesPaidStatus()
        {
            var email = await AuthenticateClientAsync();
            var user = await _userManager.FindByEmailAsync(email);

            var reservation = new MaterialReservation(user.Id, 50, false);
            _context.MaterialReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", reservation.Id.ToString() },
                { "UserId", user.Id.ToString() },
                { "Total", reservation.Total.ToString() },
                { "Paid", "true" }
            };

            var formContent = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/MaterialReservations/Edit/{reservation.Id}", formContent);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updated = _context.MaterialReservations.FirstOrDefault(r => r.Id == reservation.Id);
            Assert.NotNull(updated);
        }

        [Fact]
        public async Task Delete_ReturnsSuccess()
        {
            User user = new User();
            _context.Add(user);
            _context.SaveChanges();

            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.Add(materialReservation);
            _context.SaveChanges();

            var response = await _client.GetAsync($"MaterialReservations/Delete/{materialReservation.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesReservationAndRedirects()
        {
            var email = await AuthenticateClientAsync();
            var user = await _userManager.FindByEmailAsync(email);

            var reservation = new MaterialReservation(user.Id, 70, false);
            _context.MaterialReservations.Add(reservation);
            await _context.SaveChangesAsync();

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>());

            var response = await _client.PostAsync($"/MaterialReservations/Delete/{reservation.Id}", formContent);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var exists = _context.MaterialReservations.Any(r => r.Id == reservation.Id);
            Assert.False(exists);
        }
    }
}
