using Microsoft.AspNetCore.Hosting;
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
        private readonly TFMGoSkiDbContext _context;

        public InstructorsControllerTest(WebApplicationFactory<Program> factory)
        {
            var webApp = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });

            _client = webApp.CreateClient();

            var scope = webApp.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();
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
        public async Task Test_Instructors_Edit_Post_IdNotFound()
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

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
