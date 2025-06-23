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
    public class ReservationTimeRangeMaterialsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public ReservationTimeRangeMaterialsControllerTest(WebApplicationFactory<Program> factory)
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
        public async Task Index_ReturnsViewWithDtoList()
        {
            City city = new City("ciudad");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            // Arrange
            var material = new Material("Boots", "Boots Description", 10, 12.1m, "small", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();
             
            var timeRange = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                DateOnly.FromDateTime(DateTime.Today).AddDays(2),
                new TimeOnly(10, 0),
                new TimeOnly(12, 0),
                5,
                material.Id);
            _context.ReservationTimeRangeMaterials.Add(timeRange);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/ReservationTimeRangeMaterials");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        }

        [Fact]
        public async Task Details_ValidId_ReturnsView()
        {
            var material = _context.Materials.FirstOrDefault();
             
            var rtrm = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                DateOnly.FromDateTime(DateTime.Today).AddDays(2),
                new TimeOnly(10, 0),
                new TimeOnly(12, 0),
                10,
                material.Id);
            _context.ReservationTimeRangeMaterials.Add(rtrm);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeMaterials/Details/{rtrm.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/ReservationTimeRangeMaterials/Details/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_Create_ReturnsView()
        {
            var material = _context.Materials.FirstOrDefault();
            var response = await _client.GetAsync($"/ReservationTimeRangeMaterials/Create?materialId={material.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_Create_ValidData_CreatesAndRedirects()
        {
            var material = _context.Materials.FirstOrDefault();

            var formData = new Dictionary<string, string>
    {
        { "StartDateOnly", DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd") },
        { "EndDateOnly", DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd") },
        { "StartTimeOnly", "10:00" },
        { "EndTimeOnly", "12:00" },
        { "MaterialId", material.Id.ToString() }
    };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync($"/ReservationTimeRangeMaterials/Create?materialId={material.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_Edit_ReturnsView()
        {
            var material = _context.Materials.FirstOrDefault();
             
            var rtrm = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                DateOnly.FromDateTime(DateTime.Today).AddDays(2),
                new TimeOnly(10, 0),
                new TimeOnly(12, 0),
                5,
                material.Id);

            _context.ReservationTimeRangeMaterials.Add(rtrm);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeMaterials/Edit/{rtrm.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_Edit_ValidData_UpdatesAndRedirects()
        {
            #region crear mterial

            City city = new City("ciudad");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            // Arrange
            var material = new Material("Boots", "Boots Description", 10, 12.1m, "small", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            #endregion

            var rtrm = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                DateOnly.FromDateTime(DateTime.Today).AddDays(2),
                new TimeOnly(10, 0),
                new TimeOnly(12, 0),
                7,
                material.Id);
            _context.ReservationTimeRangeMaterials.Add(rtrm);
            await _context.SaveChangesAsync();

            var form = new Dictionary<string, string>
    {
        { "Id", rtrm.Id.ToString() },
        { "StartDateOnly", rtrm.StartDateOnly.ToString("yyyy-MM-dd") },
        { "EndDateOnly", rtrm.EndDateOnly.ToString("yyyy-MM-dd") },
        { "StartTimeOnly", rtrm.StartTimeOnly.ToString("HH:mm") },
        { "EndTimeOnly", rtrm.EndTimeOnly.ToString("HH:mm") },
        { "RemainingMaterialsQuantity", "5" },
        { "MaterialId", material.Id.ToString() }
    };
            var response = await _client.PostAsync($"/ReservationTimeRangeMaterials/Edit/{rtrm.Id}", new FormUrlEncodedContent(form));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_Delete_ReturnsView()
        {
            var material = _context.Materials.FirstOrDefault();
             
            var rtrm = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                DateOnly.FromDateTime(DateTime.Today).AddDays(2),
                new TimeOnly(10, 0),
                new TimeOnly(12, 0),
                3,
                material.Id);
            _context.ReservationTimeRangeMaterials.Add(rtrm);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeMaterials/Delete/{rtrm.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_DeleteConfirmed_DeletesAndRedirects()
        {
            #region crear mterial

            City city = new City("ciudad");
            MaterialType materialType = new MaterialType("material type");
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();
            // Arrange
            var material = new Material("Boots", "Boots Description", 10, 12.1m, "small", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            #endregion

            var rtrm = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                DateOnly.FromDateTime(DateTime.Today).AddDays(2),
                new TimeOnly(10, 0),
                new TimeOnly(12, 0),
                3,
                material.Id); 
            _context.ReservationTimeRangeMaterials.Add(rtrm);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ReservationTimeRangeMaterials/Delete/{rtrm.Id}", new FormUrlEncodedContent(new Dictionary<string, string>()));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


    }
}
