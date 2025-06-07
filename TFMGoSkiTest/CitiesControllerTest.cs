using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSkiTest
{
    public class CitiesControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public CitiesControllerTest(WebApplicationFactory<Program> factory)
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
            // Crear un nuevo usuario en la base de datos
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            // Crear rol si no existe
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = "testuser@example.com";
            var testPassword = "Test123!";

            var user = await userManager.FindByEmailAsync(testEmail);
            if (user == null)
            {
                user = new User
                {
                    UserName = testEmail,
                    Email = testEmail,
                    FullName = "Test User",
                    PhoneNumber = "123456789"
                };

                await userManager.CreateAsync(user, testPassword);
                await userManager.AddToRoleAsync(user, role);
            }

            // Simular login real con el HttpClient
            var loginData = new Dictionary<string, string>
            {
                { "UserName", testEmail },
                { "Password", testPassword }
            };

            var loginContent = new FormUrlEncodedContent(loginData);

            // Forzar cookies y redirección para mantener autenticación
            var response = await _client.PostAsync("/Account/Login", loginContent);
            response.EnsureSuccessStatusCode(); // si falla, lanza excepción
        }


        [Fact]
        public async Task Test_Cities_Index_ReturnsSuccess()
        {
            // Agregar ciudad en la base de datos en memoria
            _context.Cities.Add(new City("Test City"));
            await _context.SaveChangesAsync();

            // Realizar la solicitud GET
            var response = await _client.GetAsync("/Cities");

            // Verificar que la respuesta sea 200 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Details_ReturnsSuccess()
        {
            // Agregar ciudad en la base de datos en memoria
            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            // Realizar la solicitud GET para los detalles
            var response = await _client.GetAsync("/Cities/Details/1");

            // Verificar que la respuesta sea 200 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Details_Id_Null()
        {
            var response = await _client.GetAsync($"/Cities/Details?");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Details_Id_NotFound()
        {
            var response = await _client.GetAsync("/Cities/Details/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Create_ReturnsSuccess()
        {
            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            CityViewModel viewModel = new CityViewModel()
            {
                Name = "Test City"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Cities/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Create_ReturnsView()
        {
            CityViewModel viewModel = new CityViewModel()
            {
            };

            var json = System.Text.Json.JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Cities/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Create_ReturnsSuccess_Initial()
        {
            var response = await _client.GetAsync("/Cities/Create");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Edit_Id_ReturnsSuccess()
        {
            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Cities/Edit/{city.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Edit_Id_Zero()
        {
            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Cities/Edit?");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Edit_Id_False()
        {
            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Cities/Edit/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Edit_ReturnsSuccess()
        {
            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            CityViewModel viewModel = new CityViewModel()
            {
                Name = "Test City Update"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/Cities/Edit/{city.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Edit_InvalidModel()
        {
            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            CityViewModel viewModel = new CityViewModel()
            {
            };

            var json = System.Text.Json.JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/Cities/Edit/{city.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Edit_Id_ReturnsCityNotFound()
        { 
            var formData = new Dictionary<string, string>
            {
                { "Name", "Test City Update" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/Cities/Edit?id=0", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Edit_ReturnsCityDoesntExist()
        {
            var formData = new Dictionary<string, string>
            {
                { "Name", "Test City Update" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/Cities/Edit/999", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Delete_ReturnsSuccess()
        {
            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var response = await _client.DeleteAsync($"/Cities/Delete/{city.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Delete_Null()
        { 
            var response = await _client.DeleteAsync($"/Cities/Delete?");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Delete_NotFound()
        {
            await AuthenticateAsync();

            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var response = await _client.DeleteAsync($"/Cities/Delete/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Cities_Delete_ReturnsOk()
        {
            var city = new City("Test City");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/Cities/Delete/{city.Id}", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
