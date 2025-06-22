using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using TFMGoSki;
using Xunit;

namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class HomeControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public HomeControllerTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Test_Home_Index_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Home_Privacy_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/Home/Privacy");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Home_Error_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/Home/Error");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
