using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSkiTest
{
    public class InstructorsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public InstructorsControllerTest(WebApplicationFactory<Program> factory)
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
        public async Task Test_Instructors_Index_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/Instructors");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Details_ReturnsNotFound_ForInvalidId()
        {
            var response = await _client.GetAsync("/Instructors/Details/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Details_ReturnsNotFound_IdNull()
        {
            var response = await _client.GetAsync("/Instructors/Details?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Details_ReturnsSuccess()
        {
            var instructor = new Instructor("Test Instructor");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Instructors/Details/{instructor.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Create_Get_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/Instructors/Create");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Create_Post_RedirectsToIndex()
        {
            var formData = new Dictionary<string, string>
            {
                { "Name", "New Instructor" }
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/Instructors/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Create_Post_InvalidModel()
        {
            var viewModel = new InstructorViewModel
            {
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Instructors/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Edit_Get_ReturnsSuccess()
        {
            var instructor = new Instructor("Edit Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Instructors/Edit/{instructor.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Edit_Get_IdNotFound()
        {
            var instructor = new Instructor("Edit Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Instructors/Edit/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Edit_Get_IdNull()
        {
            var instructor = new Instructor("Edit Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Instructors/Edit?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Edit_Post_Redirects()
        {
            var instructor = new Instructor("Edit Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var viewModel = new InstructorViewModel
            {
                Id = instructor.Id,
                Name = "Updated Instructor"
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/Instructors/Edit/{instructor.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Edit_Post_InvalidModel()
        {
            var instructor = new Instructor("Edit Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var viewModel = new InstructorViewModel
            {
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/Instructors/Edit/{instructor.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Edit_Post_IdInstructorDoesntExist()
        {
            var instructor = new Instructor("Edit Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Name", "Updated Instructor" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/Instructors/Edit/999", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Delete_Get_ReturnsSuccess()
        {
            var instructor = new Instructor("Delete Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Instructors/Delete/{instructor.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Delete_Get_NotFound_Zero()
        {
            var instructor = new Instructor("Delete Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Instructors/Delete?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Delete_Get_NotFound()
        {
            var instructor = new Instructor("Delete Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Instructors/Delete/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Delete_Post_Redirects()
        {
            var instructor = new Instructor("Delete Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/Instructors/Delete/{instructor.Id}", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Instructors_Delete_Post_NotFound()
        {
            var instructor = new Instructor("Delete Test");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/Instructors/Delete/999", new StringContent(""));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
