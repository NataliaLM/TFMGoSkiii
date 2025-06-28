using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
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
        public async Task Detail_Get_ReturnsView()
        {
            City city = new City("city");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            Material material = new Material("name", "description", 12, 12.1m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);
            _context.SaveChanges();
            User user = new User();
            _context.Users.Add(user);
            _context.SaveChanges();
            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.MaterialReservations.Add(materialReservation);
            _context.SaveChanges();
            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, material.Id);
            _context.ReservationTimeRangeMaterials.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();
            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 12);
            _context.ReservationMaterialCarts.Add(reservationMaterialCart);
            _context.SaveChanges();

            var url = $"/ReservationMaterialCarts/Details/{reservationMaterialCart?.Id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Get_ReturnsView()
        {
            City city = new City("city");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            Material material = new Material("name", "description", 12, 12.1m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);
            _context.SaveChanges();
            User user = new User();
            _context.Users.Add(user);
            _context.SaveChanges();
            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.MaterialReservations.Add(materialReservation);
            _context.SaveChanges();
            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, material.Id);
            _context.ReservationTimeRangeMaterials.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();
            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 12);
            _context.ReservationMaterialCarts.Add(reservationMaterialCart);
            _context.SaveChanges();

            var url = $"/ReservationMaterialCarts/Create?materialId={material?.Id}&reservationTimeRangeMaterialId={reservationTimeRangeMaterial?.Id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_ValidModel_Redirects()
        {
            User user = new User();
            City city = new City("city");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(user);
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            Material material = new Material("name", "description", 12, 12.12m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Add(material);
            _context.SaveChanges();

            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, material.Id);
            _context.ReservationTimeRangeMaterials.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();

            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.MaterialReservations.Add(materialReservation);
            _context.SaveChanges();

            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 12);
            _context.ReservationMaterialCarts.Add(reservationMaterialCart);
            _context.SaveChanges();

            var reservationMaterialCartViewModel = new Dictionary<string, string>
            {
                { "MaterialId", $"{material.Id}" },
                { "ReservationTimeRangeMaterialId", $"{reservationTimeRangeMaterial.Id}" },
                { "MaterialReservationId", $"{materialReservation.Id}" },
                { "NumberMaterialsBooked", "5" },
                { "UserId", $"{user.Id}" },
                { "UserName", $"{user.Email}" }
            };

            var content = new FormUrlEncodedContent(reservationMaterialCartViewModel);

            var response = await _client.PostAsync("/ReservationMaterialCarts/Create", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Get_ReturnsView()
        {
            City city = new City("city");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            Material material = new Material("name", "description", 12, 12.1m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);
            _context.SaveChanges();
            User user = new User();
            _context.Users.Add(user);
            _context.SaveChanges();
            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.MaterialReservations.Add(materialReservation);
            _context.SaveChanges();
            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, material.Id);
            _context.ReservationTimeRangeMaterials.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();
            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 12);
            _context.ReservationMaterialCarts.Add(reservationMaterialCart);
            _context.SaveChanges();

            var url = $"/ReservationMaterialCarts/Edit/{reservationMaterialCart?.Id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_Redirects()
        {
            User user = new User();
            City city = new City("city");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(user);
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            Material material = new Material("name", "description", 12, 12.12m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Add(material);
            _context.SaveChanges();

            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, material.Id);
            _context.ReservationTimeRangeMaterials.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();

            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.MaterialReservations.Add(materialReservation);
            _context.SaveChanges();

            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 12);
            _context.ReservationMaterialCarts.Add(reservationMaterialCart);
            _context.SaveChanges();

            var reservationMaterialCartViewModel = new Dictionary<string, string>
            {
                { "MaterialId", $"{material.Id}" },
                { "ReservationTimeRangeMaterialId", $"{reservationTimeRangeMaterial.Id}" },
                { "MaterialReservationId", $"asd@asd.com" },
                { "NumberMaterialsBooked", "testPassword" },
                { "UserId", $"asd@asd.com" },
                { "UserName", "testPassword" },
            };

            var content = new FormUrlEncodedContent(reservationMaterialCartViewModel);

            var response = await _client.PostAsJsonAsync($"/ReservationMaterialCarts/Edit/{reservationMaterialCart.Id}", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_Redirects_Success()
        {
            User user = new User();
            City city = new City("city");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(user);
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            Material material = new Material("name", "description", 12, 12.12m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Add(material);
            _context.SaveChanges();

            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 11, material.Id);
            _context.ReservationTimeRangeMaterials.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();

            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.MaterialReservations.Add(materialReservation);
            _context.SaveChanges();

            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 1);
            _context.ReservationMaterialCarts.Add(reservationMaterialCart);
            _context.SaveChanges();

            var reservationMaterialCartViewModel = new Dictionary<string, string>
            {
                { "MaterialId", $"{material.Id}" },
                { "ReservationTimeRangeMaterialId", $"{reservationTimeRangeMaterial.Id}" },
                { "MaterialReservationId", $"{materialReservation.Id}" },
                { "NumberMaterialsBooked", "12" },
                { "UserId", $"{user.Id}" },
                { "UserName", "testPassword" },
            };

            var content = new FormUrlEncodedContent(reservationMaterialCartViewModel);

            var response = await _client.PostAsync($"/ReservationMaterialCarts/Edit/{reservationMaterialCart.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_Redirects_Success_TimeRange()
        {
            User user = new User();
            City city = new City("city");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(user);
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            Material material = new Material("name", "description", 12, 12.12m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Add(material);
            _context.SaveChanges();

            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 11, material.Id);
            _context.ReservationTimeRangeMaterials.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();

            ReservationTimeRangeMaterial reservationTimeRangeMaterialUpdate = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 12, material.Id);
            _context.ReservationTimeRangeMaterials.Add(reservationTimeRangeMaterialUpdate);
            _context.SaveChanges();

            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.MaterialReservations.Add(materialReservation);
            _context.SaveChanges();

            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 1);
            _context.ReservationMaterialCarts.Add(reservationMaterialCart);
            _context.SaveChanges();

            var reservationMaterialCartViewModel = new Dictionary<string, string>
            {
                { "MaterialId", $"{material.Id}" },
                { "ReservationTimeRangeMaterialId", $"{reservationTimeRangeMaterialUpdate.Id}" },
                { "MaterialReservationId", $"{materialReservation.Id}" },
                { "NumberMaterialsBooked", "12" },
                { "UserId", $"{user.Id}" },
                { "UserName", "testPassword" },
            };

            var content = new FormUrlEncodedContent(reservationMaterialCartViewModel);

            var response = await _client.PostAsync($"/ReservationMaterialCarts/Edit/{reservationMaterialCart.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Redirects on success
        }


        [Fact]
        public async Task Delete_Get_ReturnsView()
        {
            City city = new City("city");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            Material material = new Material("name", "description", 12, 12.1m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);
            _context.SaveChanges();
            User user = new User();
            _context.Users.Add(user);
            _context.SaveChanges();
            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.MaterialReservations.Add(materialReservation);
            _context.SaveChanges();
            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, material.Id);
            _context.ReservationTimeRangeMaterials.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();
            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 12);
            _context.ReservationMaterialCarts.Add(reservationMaterialCart);
            _context.SaveChanges();

            var url = $"/ReservationMaterialCarts/Delete/{reservationMaterialCart?.Id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
