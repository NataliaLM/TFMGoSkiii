using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TFMGoSki.Data;
using TFMGoSki.Models;

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
    }
}
