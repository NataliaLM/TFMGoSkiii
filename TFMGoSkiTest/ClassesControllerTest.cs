using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class ClassesControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public ClassesControllerTest(WebApplicationFactory<Program> factory)
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
            var userManager = _factory.Services.GetRequiredService<UserManager<User>>();
            var roleManager = _factory.Services.GetRequiredService<RoleManager<Role>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }

            var testEmail = $"testuser{Guid.NewGuid()}Classes@exampleClasses.com"; // email único
            var testPassword = "Test123!";

            var user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                FullName = "Test User Classes",
                PhoneNumber = "133456789"
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
        public async Task Test_Classes_Index_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/Classes");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Details_ReturnsView()
        {
            Instructor instructor = new Instructor("Roberto Rodriguez");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            var response = await _client.GetAsync($"/Classes/Details/{@class.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Details_ReturnsNotFound_WhenIdInvalid()
        {
            var response = await _client.GetAsync("/Classes/Details/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Details_ReturnsNotFound_NotFound_Zero()
        {
            var response = await _client.GetAsync("/Classes/Details?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Create_Get_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/Classes/Create");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Create_Post_ReturnsRedirect()
        {
            var instructor = new Instructor("Jorge Poneros");
            _context.Instructors.Add(instructor);

            var city = new City("Madrid");
            _context.Cities.Add(city);

            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Name", "Test Class" },
                { "Price", "30" },
                { "StudentQuantity", "20" },
                { "ClassLevel", "2" }, // Enum: Advanced = 2 (ajusta si es diferente)
                { "Instructor", instructor.Id.ToString() },
                { "City", city.Id.ToString() }
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/Classes/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task Test_Classes_Create_Post_ReturnsSuccess()
        {
            var instructor = new Instructor("Jorge Poneros");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            ClassViewModel classViewModel = new ClassViewModel()
            {
            };

            var json = System.Text.Json.JsonSerializer.Serialize(classViewModel);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Classes/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Edit_ReturnSuccess()
        {
            var instructor = new Instructor("Jorge Poneros");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var city = new City("Mexico");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var @class = new Class("Name Class", 15, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Classes/Edit/{@class.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Edit_ReturnsNotFound_ForInvalidId()
        {
            var response = await _client.GetAsync("/Classes/Edit/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Edit_ReturnsNotFound_NotFound_Zero()
        {
            var response = await _client.GetAsync("/Classes/Edit?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Edit_Post_ReturnsRedirect()
        {
            var instructor = new Instructor("Jorge Poneros");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var city = new City("Mexico");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Class Name", 140, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            ClassViewModel classViewModel = new ClassViewModel()
            {
                Name = "Test Class Update",
                Price = 31,
                StudentQuantity = 21,
                ClassLevel = ClassLevel.Advanced,
                Instructor = instructor.Id,
                City = city.Id
            };

            var json = System.Text.Json.JsonSerializer.Serialize(classViewModel);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/Classes/Edit/{@class.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Edit_Post_ReturnsNotFound()
        {
            var instructor = new Instructor("Jorge Poneros");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var city = new City("Mexico");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Class Name", 140, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Name", "Test Class Update" },
                { "Price", "31" },
                { "StudentQuantity", "21" },
                { "ClassLevel", ((int)ClassLevel.Advanced).ToString() },
                { "Instructor", instructor.Id.ToString() },
                { "City", city.Id.ToString() }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/Classes/Edit/999", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Edit_Post_ReturnsRedirect_InvalidModel()
        {
            var instructor = new Instructor("Jorge Poneros");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var city = new City("Mexico");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Class Name", 140, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();


            ClassViewModel classViewModel = new ClassViewModel()
            {
            };

            var json = System.Text.Json.JsonSerializer.Serialize(classViewModel);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/Classes/Edit/{@class.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Delete_ReturnsNotFound()
        {
            var instructor = new Instructor("Jorge Poneros");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var city = new City("Mexico");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var @class = new Class("Name Class", 15, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Classes/Delete/{@class.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Delete_ReturnsNotFound_ForInvalidId()
        {
            var response = await _client.GetAsync("/Classes/Delete/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Delete_ReturnsNotFound_NullId()
        {
            var response = await _client.GetAsync("/Classes/Delete?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Delete_Post_ReturnsSuccess()
        {
            var instructor = new Instructor("Jorge Poneros");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var city = new City("Mexico");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var @class = new Class("Name Class", 15, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/Classes/Delete/{@class.Id}", new StringContent(""));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Classes_Delete_Post_NotFound()
        {
            var instructor = new Instructor("Jorge Poneros");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            var city = new City("Mexico");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var @class = new Class("Name Class", 15, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/Classes/Delete/999", new StringContent(""));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
