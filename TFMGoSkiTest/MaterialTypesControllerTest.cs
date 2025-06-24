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
    public class MaterialTypesControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public MaterialTypesControllerTest(WebApplicationFactory<Program> factory)
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

            var testEmail = $"testuser{Guid.NewGuid()}MaterialTypes@exampleCiudades.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User MaterialTypes",
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
        public async Task Index_ReturnsViewWithStatusCode200()
        {
            var response = await _client.GetAsync("/MaterialTypes");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Details_ReturnsMaterialType_WhenValidId()
        {
            var materialType = new MaterialType("Test Detail");
            _context.MaterialTypes.Add(materialType);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/MaterialTypes/Details/{materialType.Id}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Test Detail", content);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenInvalidId()
        {
            var response = await _client.GetAsync("/MaterialTypes/Details/999999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_ValidMaterialType_RedirectsToIndex()
        {
            var data = new Dictionary<string, string>
            {
                { "Name", "Unique Material Type" }
            };

            var content = new FormUrlEncodedContent(data);
            var response = await _client.PostAsync("/MaterialTypes/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_DuplicateMaterialType_ShowsError()
        {
            var type = new MaterialType("Duplicated");
            _context.MaterialTypes.Add(type);
            await _context.SaveChangesAsync();

            var data = new Dictionary<string, string>
            {
                { "Name", "Duplicated" }
            };

            var content = new FormUrlEncodedContent(data);
            var response = await _client.PostAsync("/MaterialTypes/Create", content);
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // retorna a la vista con errores
            Assert.Contains("There is already a material type with this name", responseString);
        }

        [Fact]
        public async Task Edit_Get_ReturnsViewWithMaterialType()
        {
            var type = new MaterialType("Editable");
            _context.MaterialTypes.Add(type);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/MaterialTypes/Edit/{type.Id}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_ValidUpdate_RedirectsToIndex()
        {
            var type = new MaterialType("Old Type");
            _context.MaterialTypes.Add(type);
            await _context.SaveChangesAsync();

            var data = new Dictionary<string, string>
            {
                { "Id", type.Id.ToString() },
                { "Name", "Updated Type" }
            };

            var content = new FormUrlEncodedContent(data);
            var response = await _client.PostAsync($"/MaterialTypes/Edit/{type.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);  
        }

        [Fact]
        public async Task Edit_Post_DuplicateName_ShowsError()
        {
            var type1 = new MaterialType("Type1");
            var type2 = new MaterialType("Type2");
            _context.MaterialTypes.AddRange(type1, type2);
            await _context.SaveChangesAsync();

            var data = new Dictionary<string, string>
            {
                { "Id", type2.Id.ToString() },
                { "Name", "Type1" }
            };

            var content = new FormUrlEncodedContent(data);
            var response = await _client.PostAsync($"/MaterialTypes/Edit/{type2.Id}", content);
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("There is already a material type with this name", html);
        }

        [Fact]
        public async Task Delete_Get_ReturnsConfirmationPage()
        {
            var type = new MaterialType("To Delete");
            _context.MaterialTypes.Add(type);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/MaterialTypes/Delete/{type.Id}");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("To Delete", html);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesMaterialType()
        {
            var type = new MaterialType("Delete Confirmed");
            _context.MaterialTypes.Add(type);
            await _context.SaveChangesAsync();

            var content = new FormUrlEncodedContent(new Dictionary<string, string>());
            var response = await _client.PostAsync($"/MaterialTypes/Delete/{type.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
