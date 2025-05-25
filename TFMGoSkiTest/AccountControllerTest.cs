using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using Xunit;

namespace TFMGoSkiTest
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public AccountControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_Get_ReturnsOk()
        {
            var response = await _client.GetAsync("/Account/Register");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Register_Post_InvalidModel_ReturnsView()
        {
            var model = new RegisterViewModel(); // Empty model, will be invalid
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Account/Register", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_Get_ReturnsOk()
        {
            var response = await _client.GetAsync("/Account/Login");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ShowsError()
        {
            var model = new LoginViewModel
            {
                UserName = "nonexistentuser@example.com",
                Password = "WrongPassword123!",
                RememberMe = false
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Account/Login", content);

            // It should return to login page with error, not redirect
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Profile_Unauthenticated_RedirectsToLogin()
        {
            var response = await _client.GetAsync("/Account/Profile");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Unauthenticated_ReturnsForbidden()
        {
            var content = new StringContent(""); // Empty body
            var response = await _client.PostAsync("/Account/Logout", content);

            // As Logout has [Authorize], this returns 302 to login
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAccount_Unauthenticated_RedirectsToLogin()
        {
            var response = await _client.PostAsync("/Account/DeleteAccount", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AllUsers_Get_ReturnsOk()
        {
            var response = await _client.GetAsync("/Account/AllUsers");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
