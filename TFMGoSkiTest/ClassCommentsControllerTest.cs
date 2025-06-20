using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TFMGoSki.Controllers;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;



namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class ClassCommentsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;
        private readonly ClassCommentsController _controller;
        private readonly UserManager<User> _userManager;
        public ClassCommentsControllerTest(WebApplicationFactory<Program> factory)
        {            
            _factory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Test"));
            _client = _factory.CreateClient();

            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();

            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _controller = new ClassCommentsController(_context, _userManager);


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

        private async Task AuthenticateClientAsync(string role = "Client")
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
        public async Task Index_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            _context.ClassComments.Add(new ClassComment(classReservation.Id, "Great class", 5));
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync("/ClassComments");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task IndexClient_ReturnsSuccess()
        {
            var contentClient = new FormUrlEncodedContent(new Dictionary<string, string> { });

            var responseClient = await _client.PostAsync("/Account/Logout", contentClient);
            responseClient.EnsureSuccessStatusCode();

            AuthenticateClientAsync();

            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            _context.ClassComments.Add(new ClassComment(classReservation.Id, "Great class", 5));
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync("/ClassComments/IndexUser");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Details_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassComments/Details/{classComment.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Details_IdNull_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/ClassComments/Details");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync("/ClassComments/Create");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_Valid_ReturnsRedirect()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "classReservationId", "1" },
                { "Text", "Great lesson!" },
                { "Raiting", "5" }
            };
            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/ClassComments/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_Invalid()
        {
            var formData = new Dictionary<string, string>
            {
            };
            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/ClassComments/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassComments/Edit/{classComment.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Get_NotFound()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassComments/Edit?");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_Valid_UpdatesEntry()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Original comment", 3);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Id", classComment.Id.ToString() },
                { "ClassReservationId", classReservation.Id.ToString() },
                { "Text", "Updated comment" },
                { "Raiting", "4" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ClassComments/Edit/{classComment.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ClassComments/Delete/{classComment.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Get_Invalid()
        {
            var response = await _client.GetAsync($"/ClassComments/Delete?");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Post_RemovesEntry()
        {
            Instructor instructor = new Instructor("InstructorName");
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            City city = new City("Astún y Candanchú (Huesca)");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            Class @class = new Class("Ski Basics", 12.12m, 12, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = "email@user.com",
                FullName = "Full Name",
                Email = "email@user.com",
                PhoneNumber = "123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Now.AddHours(1)), TimeOnly.FromDateTime(DateTime.Now.AddHours(2)), 8, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            ClassReservation classReservation = new ClassReservation(user.Id, @class.Id, reservationTimeRangeClass.Id, 1);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            ClassComment classComment = new ClassComment(classReservation.Id, "Great class", 5);
            _context.ClassComments.Add(classComment);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ClassComments/Delete/{classComment.Id}", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        /**/
        [Fact]
        public async Task Index_ShouldSkipClassComment_WhenClassReservationIsNull()
        {
            // Arrange: agregar un comentario que referencia un ClassReservation inexistente
            var comment = new ClassComment(1000, "Test", 3);
            _context.ClassComments.Add(comment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ClassCommentDto>>(viewResult.Model);
            Assert.DoesNotContain(model, c => c.Id == comment.Id);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenClassCommentIsNull()
        {
            // Arrange: id que no existe
            int invalidId = 999999;

            // Act
            var result = await _controller.Details(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        //[Fact]
        //public async Task Create_Post_ReturnsViewWithModelError_WhenCommentAlreadyExists()
        //{
        //    Arrange
        //   var existingReservation = new ClassReservation(1, 1, 1, 1);
        //    var existingComment = new ClassComment(1, "Existente", 4);
        //    _context.ClassReservations.Add(existingReservation);
        //    _context.ClassComments.Add(existingComment);
        //    await _context.SaveChangesAsync();

        //    var viewModel = new ClassCommentViewModel { ClassReservationId = existingReservation.Id, Text = "Nuevo comentario", Raiting = 5 };

        //    Act
        //   var result = await _controller.Create(existingReservation.Id, viewModel);

        //    Assert
        //   var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.False(_controller.ModelState.IsValid);
        //    Assert.True(_controller.ModelState.ErrorCount > 0);
        //}

        [Fact]
        public async Task Create_Post_RedirectsToIndexUser_WhenUserIsClient()
        {
            // Arrange
            var classReservation = new ClassReservation(2, 2, 2, 2);
            _context.ClassReservations.Add(classReservation);
            await _context.SaveChangesAsync();

            var viewModel = new ClassCommentViewModel { ClassReservationId = classReservation.Id, Text = "Comentario nuevo", Raiting = 5 };

            // Simular usuario con rol Client
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, "userClient"),
        new Claim(ClaimTypes.Role, "Client")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.Create(classReservation.Id, viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("IndexUser", redirectResult.ActionName);
        }

        //[Fact]
        //public async Task Edit_Post_UpdatesAndRedirects_WhenModelIsValid()
        //{
        //    // Arrange
        //    var classComment = new ClassComment(1, "Texto viejo", 3);
        //    _context.ClassComments.Add(classComment);
        //    await _context.SaveChangesAsync();

        //    var viewModel = new ClassCommentViewModel { Id = classComment.Id, ClassReservationId = 1, Text = "Texto nuevo", Raiting = 4 };

        //    // Act
        //    var result = await _controller.Edit(classComment.Id, viewModel);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.True(redirectResult.ActionName == "Index" || redirectResult.ActionName == "IndexUser");

        //    // Verificar que se haya actualizado
        //    var updated = _context.ClassComments.First(c => c.Id == classComment.Id);
        //    Assert.Equal("Texto nuevo", updated.Text);
        //    Assert.Equal(4, updated.Raiting);
        //}

    }
}
