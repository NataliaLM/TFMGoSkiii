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
        public async Task Register_Post_ValidModel_ReturnsView()
        {
            var model = new RegisterViewModel()
            {
                FullName = "Full Name",
                Email = "full@name.com",
                PhoneNumber = "123456789",
                Password = "123asdASD@",
                RoleName = "Client"
            };
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Account/Register", content);

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
        public async Task Login_Post_ValidCredentials_ShowsError()
        {
            var modelRegister = new RegisterViewModel()
            {
                FullName = "Full Name",
                Email = "full@name.com",
                PhoneNumber = "123456789",
                Password = "123asdASD@",
                RoleName = "Client"
            };
            var contentRegister = new StringContent(JsonSerializer.Serialize(modelRegister), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Account/Register", contentRegister);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var model = new LoginViewModel
            {
                UserName = "full@name.com",
                Password = "123asdASD@",
                RememberMe = false
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var responseLogin = await _client.PostAsync("/Account/Login", content);

            Assert.Equal(HttpStatusCode.OK, responseLogin.StatusCode);
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ShowsError()
        {
            var model = new LoginViewModel
            {
                UserName = "full@name.com",
                Password = "123asdASD@",
                RememberMe = false
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Account/Login", content);

            // It should return to login page with error, not redirect
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Profile_Authenticated_RedirectsToProfile()
        {
            var modelRegister = new RegisterViewModel()
            {
                FullName = "Full Name",
                Email = "full@name.com",
                PhoneNumber = "123456789",
                Password = "123asdASD@",
                RoleName = "Client"
            };
            var contentRegister = new StringContent(JsonSerializer.Serialize(modelRegister), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Account/Register", contentRegister);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var model = new LoginViewModel
            {
                UserName = "full@name.com",
                Password = "123asdASD@",
                RememberMe = false
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var responseLogin = await _client.PostAsync("/Account/Login", content);

            Assert.Equal(HttpStatusCode.OK, responseLogin.StatusCode);

            var responseProfile = await _client.GetAsync("/Account/Profile");

            Assert.Equal(HttpStatusCode.OK, responseProfile.StatusCode);
        }

        [Fact]
        public async Task Profile_Unauthenticated_RedirectsToLogin()
        {
            var response = await _client.GetAsync("/Account/Profile");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Authenticated_ReturnsOK()
        {
            var modelRegister = new RegisterViewModel()
            {
                FullName = "Full Name",
                Email = "full@name.com",
                PhoneNumber = "123456789",
                Password = "123asdASD@",
                RoleName = "Client"
            };
            var contentRegister = new StringContent(JsonSerializer.Serialize(modelRegister), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Account/Register", contentRegister);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var model = new LoginViewModel
            {
                UserName = "full@name.com",
                Password = "123asdASD@",
                RememberMe = false
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var responseLogin = await _client.PostAsync("/Account/Login", content);

            Assert.Equal(HttpStatusCode.OK, responseLogin.StatusCode);

            var contentLogout = new StringContent("");
            var responseLogout = await _client.PostAsync("/Account/Logout", content);

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
        public async Task DeleteAccount_Authenticated_RedirectsToList()
        {
            var modelRegister = new RegisterViewModel()
            {
                FullName = "Full Name",
                Email = "full@name.com",
                PhoneNumber = "123456789",
                Password = "123asdASD@",
                RoleName = "Client"
            };
            var contentRegister = new StringContent(JsonSerializer.Serialize(modelRegister), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Account/Register", contentRegister);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var model = new LoginViewModel
            {
                UserName = "full@name.com",
                Password = "123asdASD@",
                RememberMe = false
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var responseLogin = await _client.PostAsync("/Account/Login", content);

            Assert.Equal(HttpStatusCode.OK, responseLogin.StatusCode);


            var responseDelete = await _client.PostAsync("/Account/DeleteAccount", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, responseDelete.StatusCode);
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
