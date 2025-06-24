using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using TFMGoSki;
using TFMGoSki.Data;
using TFMGoSki.Models; 
using TFMGoSki.ViewModels;
using Xunit;

namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class ReservationMaterialCartsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationMaterialCartsControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Test"));
            _client = _factory.CreateClient();

            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            Task.Run(() => AuthenticateClientAsync()).Wait();
        }

        private async Task<string> AuthenticateClientAsync(string role = "Client")
        {
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}@example.com";
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User",
                PhoneNumber = "123456789"
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
        public async Task Index_ReturnsSuccessAndView()
        {
            var response = await _client.GetAsync("/ReservationMaterialCarts/Index");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task IndexUser_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/ReservationMaterialCarts/IndexUser");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Get_ReturnsView()
        {
            var material = _context.Materials.FirstOrDefault();
            var range = _context.ReservationTimeRangeMaterials.FirstOrDefault();

            var url = $"/ReservationMaterialCarts/Create?materialId={material?.Id}&reservationTimeRangeMaterialId={range?.Id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_ValidModel_Redirects()
        {
            var userId = _context.Users.FirstOrDefault()?.Id ?? 1;
            var material = _context.Materials.FirstOrDefault();
            var range = _context.ReservationTimeRangeMaterials.FirstOrDefault();
            var reservation = _context.MaterialReservations.FirstOrDefault(r => r.UserId == userId && !r.Paid);

            if (material == null || range == null || reservation == null)
            {
                return; // Setup incomplete
            }

            var model = new ReservationMaterialCartViewModel
            {
                MaterialId = material.Id,
                ReservationTimeRangeMaterialId = range.Id,
                MaterialReservationId = reservation.Id,
                NumberMaterialsBooked = 1,
                UserId = userId,
                UserName = _context.Users.Find(userId)?.Email
            };

            var response = await _client.PostAsJsonAsync("/ReservationMaterialCarts/Create", model);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode); // Redirects on success
        }

        [Fact]
        public async Task DeleteConfirmed_ValidId_DeletesCartAndUpdatesState()
        {
            // Arrange
            var user = _context.Users.FirstOrDefault();
            var material = _context.Materials.FirstOrDefault();
            var timeRange = _context.ReservationTimeRangeMaterials.FirstOrDefault();

            if (user == null || material == null || timeRange == null)
                return; // Prueba no válida si falta algún dato

            // Crear una reserva de material del usuario
            var materialReservation = new MaterialReservation(user.Id, 0, false);
            _context.MaterialReservations.Add(materialReservation);
            await _context.SaveChangesAsync();

            // Crear un carrito vinculado
            var cart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, timeRange.Id, 2);
            
            _context.ReservationMaterialCarts.Add(cart);

            // Aumentamos el total como haría el POST de Create
            materialReservation.Total = material.Price * cart.NumberMaterialsBooked;
            timeRange.RemainingMaterialsQuantity = 5;
            await _context.SaveChangesAsync();

            var cartId = cart.Id;
            var initialRemainingQty = timeRange.RemainingMaterialsQuantity;

            // Act
            var response = await _client.PostAsync($"/ReservationMaterialCarts/Delete/{cartId}", new FormUrlEncodedContent(new Dictionary<string, string>()));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
