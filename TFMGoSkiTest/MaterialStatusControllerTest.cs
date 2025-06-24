using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class MaterialStatusControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public MaterialStatusControllerTest(WebApplicationFactory<Program> factory)
        {
            // Configurar el entorno para usar la base de datos en memoria en las pruebas
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });

            // Crear cliente HTTP para realizar las pruebas
            _client = _factory.CreateClient();

            // Crear el contexto con la base de datos en memoria
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

            var testEmail = $"testuser{Guid.NewGuid()}MaterialStatus@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User MaterialStatus",
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
        public async Task Index_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/MaterialStatus");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ValidMaterialStatus_RedirectsToIndex()
        {
            var data = new Dictionary<string, string>
            {
                { "Name", "Test Status" }
            };

            var content = new FormUrlEncodedContent(data);
            var response = await _client.PostAsync("/MaterialStatus/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_DuplicateName_ShowsValidationError()
        {
            // Arrange: create one first
            var status = new MaterialStatus("Duplicate Status");
            _context.MaterialStatuses.Add(status);
            await _context.SaveChangesAsync();

            var data = new Dictionary<string, string>
            {
                { "Name", "Duplicate Status" }
            };

            var content = new FormUrlEncodedContent(data);

            // Act
            var response = await _client.PostAsync("/MaterialStatus/Create", content);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // vuelve al formulario
            Assert.Contains("There is already a material status with this name", responseString);
        }

        [Fact]
        public async Task Edit_Get_ReturnsView()
        {
            // Arrange
            var status = new MaterialStatus("Editable Status");
            _context.MaterialStatuses.Add(status);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/MaterialStatus/Edit/{status.Id}");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Editable Status", html);
        }

        [Fact]
        public async Task Edit_Post_ValidUpdate_Redirects()
        {
            var status = new MaterialStatus("Old Name");
            _context.MaterialStatuses.Add(status);
            await _context.SaveChangesAsync();

            var data = new Dictionary<string, string>
            {
                { "Id", status.Id.ToString() },
                { "Name", "Updated Name" }
            };

            var content = new FormUrlEncodedContent(data);
            var response = await _client.PostAsync($"/MaterialStatus/Edit/{status.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Get_ReturnsConfirmationView()
        {
            var status = new MaterialStatus("To Be Deleted");
            _context.MaterialStatuses.Add(status);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/MaterialStatus/Delete/{status.Id}");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("To Be Deleted", html);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesEntityAndRedirects()
        {
            var status = new MaterialStatus("To Delete");
            _context.MaterialStatuses.Add(status);
            await _context.SaveChangesAsync();

            var content = new FormUrlEncodedContent(new Dictionary<string, string>());
            var response = await _client.PostAsync($"/MaterialStatus/Delete/{status.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Details_ValidId_ReturnsStatus()
        {
            var status = new MaterialStatus("Details Test");
            _context.MaterialStatuses.Add(status);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/MaterialStatus/Details/{status.Id}");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Details Test", html);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/MaterialStatus/Details/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
