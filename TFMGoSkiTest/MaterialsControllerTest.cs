using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TFMGoSki.Controllers;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using Xunit;

namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class MaterialsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TFMGoSkiDbContext _context;

        public MaterialsControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient();

            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();

            // Limpiar la base de datos para tests
            _context.Materials.RemoveRange(_context.Materials);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsView_WithMaterials()
        {
            // Arrange
            var material = new Material("Ski", "Good ski", 10, 100, "M", 1, 1, 1);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/Materials");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Ski", content);
        }

        [Fact]
        public async Task IndexUser_FiltersWork()
        {
            // Arrange
            var material1 = new Material("Ski1", "Desc", 5, 50, "M", 1, 1, 1);
            var material2 = new Material("Ski2", "Desc", 5, 150, "L", 1, 1, 1);
            _context.Materials.AddRange(material1, material2);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/Materials/IndexUser?minPrice=100&size=L");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Ski2", content);
            Assert.DoesNotContain("Ski1", content);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdNullOrInvalid()
        {
            var responseNull = await _client.GetAsync("/Materials/Details");
            var responseInvalid = await _client.GetAsync("/Materials/Details/999999");

            Assert.Equal(HttpStatusCode.NotFound, responseNull.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, responseInvalid.StatusCode);
        }

        [Fact]
        public async Task Details_ReturnsView_WhenMaterialExists()
        {
            var material = new Material("Boots", "Desc", 5, 120, "S", 1, 1, 1);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Materials/Details/{material.Id}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Boots", content);
        }

        [Fact]
        public async Task Create_Get_ReturnsView()
        {
            var response = await _client.GetAsync("/Materials/Create");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_CreatesMaterial_WhenValid()
        {
            var formData = new Dictionary<string, string>
            {
                {"Name", "NewMaterial"},
                {"Description", "Description"},
                {"QuantityMaterial", "3"},
                {"Price", "30"},
                {"Size", "M"},
                {"CityId", "1"},
                {"MaterialTypeId", "1"},
                {"MaterialStatusId", "1"}
            };

            var response = await _client.PostAsync("/Materials/Create", new FormUrlEncodedContent(formData));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(_context.Materials.Any(m => m.Name == "NewMaterial"));
        }

        [Fact]
        public async Task Create_Post_ReturnsView_WhenDuplicateName()
        {
            _context.Materials.Add(new Material("DuplicateName", "Desc", 1, 10, "M", 1, 1, 1));
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                {"Name", "DuplicateName"},
                {"Description", "Desc"},
                {"QuantityMaterial", "3"},
                {"Price", "30"},
                {"Size", "M"},
                {"CityId", "1"},
                {"MaterialTypeId", "1"},
                {"MaterialStatusId", "1"}
            };

            var response = await _client.PostAsync("/Materials/Create", new FormUrlEncodedContent(formData));
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("There is already a material type with this name.", content);
        }

        [Fact]
        public async Task Edit_Get_ReturnsView_WhenExists()
        {
            var material = new Material("EditMaterial", "Desc", 1, 10, "M", 1, 1, 1);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Materials/Edit/{material.Id}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("EditMaterial", content);
        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFound_WhenNotExists()
        {
            var response = await _client.GetAsync("/Materials/Edit/999999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_UpdatesMaterial_WhenValid()
        {
            var material = new Material("ToEdit", "Desc", 1, 10, "M", 1, 1, 1);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                {"Id", material.Id.ToString()},
                {"Name", "EditedName"},
                {"Description", "EditedDesc"},
                {"QuantityMaterial", "5"},
                {"Price", "50"},
                {"Size", "L"},
                {"CityId", "1"},
                {"MaterialTypeId", "1"},
                {"MaterialStatusId", "1"}
            };

            var response = await _client.PostAsync($"/Materials/Edit/{material.Id}", new FormUrlEncodedContent(formData));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(_context.Materials.Any(m => m.Name == "EditedName"));
        }

        [Fact]
        public async Task Edit_Post_ReturnsView_WhenDuplicateName()
        {
            var material1 = new Material("Material1", "Desc", 1, 10, "M", 1, 1, 1);
            var material2 = new Material("Material2", "Desc", 1, 10, "M", 1, 1, 1);
            _context.Materials.AddRange(material1, material2);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                {"Id", material2.Id.ToString()},
                {"Name", "Material1"}, // Duplicate name
                {"Description", "Desc"},
                {"QuantityMaterial", "3"},
                {"Price", "30"},
                {"Size", "M"},
                {"CityId", "1"},
                {"MaterialTypeId", "1"},
                {"MaterialStatusId", "1"}
            };

            var response = await _client.PostAsync($"/Materials/Edit/{material2.Id}", new FormUrlEncodedContent(formData));
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("There is already a material type with this name.", content);
        }

        [Fact]
        public async Task Delete_Get_ReturnsView_WhenExists()
        {
            var material = new Material("ToDelete", "Desc", 1, 10, "M", 1, 1, 1);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/Materials/Delete/{material.Id}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("ToDelete", content);
        }

        [Fact]
        public async Task Delete_Get_ReturnsNotFound_WhenNotExists()
        {
            var response = await _client.GetAsync("/Materials/Delete/999999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Post_DeletesMaterial()
        {
            var material = new Material("DeleteConfirmed", "Desc", 1, 10, "M", 1, 1, 1);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string> { { "id", material.Id.ToString() } };

            var response = await _client.PostAsync($"/Materials/Delete/{material.Id}", new FormUrlEncodedContent(formData));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain(_context.Materials, m => m.Id == material.Id);
        }
    }
}
