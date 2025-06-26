using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Claims;
using TFMGoSki.Controllers;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;
using Xunit;

namespace TFMGoSkiTest
{
    [Collection("Non-Parallel Tests")]
    public class MaterialCommentsControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly TFMGoSkiDbContext _context;
        private readonly MaterialCommentsController _controller;
        private readonly UserManager<User> _userManager;

        public MaterialCommentsControllerTest(WebApplicationFactory<Program> factory)
        {
            var scope = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Test")).Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TFMGoSkiDbContext>();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _controller = new MaterialCommentsController(_context, _userManager);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1") // Simula un usuario con ID 1
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithMaterialComments()
        {
            // Arrange
            _context.MaterialComments.Add(new MaterialComment(1, "Test comment", 5));
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<MaterialComment>>(viewResult.Model);
            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task Details_Get_ReturnsViewResult_WithMaterialComments()
        {
            MaterialComment materialComment = new MaterialComment(1, "Test comment", 5);
            // Arrange
            _context.MaterialComments.Add(materialComment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(materialComment.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MaterialComment>(viewResult.Model);
        }

        [Fact]
        public async Task Details_Get_ReturnsViewResult_WithMaterialComments_IdNull()
        {
            MaterialComment materialComment = new MaterialComment(1, "Test comment", 5);
            // Arrange
            _context.MaterialComments.Add(materialComment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(null);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Get_ReturnsViewResult_WithoutMaterialComments()
        {
            MaterialComment materialComment = new MaterialComment(1, "Test comment", 5);
            
            // Act
            var result = await _controller.Details(materialComment.Id);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task IndexUser_ReturnsViewResult_WithUserComments()
        {
            // Arrange
            #region create material and time range
            var user = new User();
            _context.Users.Add(user);

            MaterialStatus materialStatus = new MaterialStatus("New");
            _context.Add(materialStatus);

            MaterialType materialType = new MaterialType("Ski");
            _context.Add(materialType);

            City city = new City("Sierra Nevada");
            _context.Add(city);

            await _context.SaveChangesAsync();

            var material = new Material("Ski", "Ski Description", 12, 12, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);

            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
            var startTime = TimeOnly.FromDateTime(DateTime.Today.AddHours(9));  // Ej: 09:00
            var endTime = TimeOnly.FromDateTime(DateTime.Today.AddHours(17));  // Ej: 17:00

            var timeRange = new ReservationTimeRangeMaterial(startDate, endDate, startTime, endTime, 12, material.Id);
            _context.ReservationTimeRangeMaterials.Add(timeRange);
            #endregion

            #region crear reservation
            MaterialReservation materialReservation = new MaterialReservation(user.Id, 120, false);
            _context.Add(materialReservation);
            await _context.SaveChangesAsync();

            var reservation = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, timeRange.Id, 12);
            _context.ReservationMaterialCarts.Add(reservation);
            await _context.SaveChangesAsync();

            _context.MaterialComments.Add(new MaterialComment(reservation.Id, "User comment", 4));
            await _context.SaveChangesAsync();
            #endregion

            // Act
            var result = await _controller.IndexUser();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<MaterialCommentDto>>(viewResult.Model);
            //Assert.Single(model);
        }

        [Fact]
        public async Task Create_Get_ReturnsViewResult_WithMaterialComments()
        {
            City city = new City("city");
            _context.Add(city);
            MaterialType materialType = new MaterialType("material type");
            _context.Add(materialType);
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(materialStatus);
            _context.SaveChanges();

            Material material = new Material("material", "description", 12, 12.12m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Add(material);

            User user = new User();
            _context.Add(user);

            _context.SaveChanges();

            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.Add(materialReservation);

            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Today.AddHours(1)), TimeOnly.FromDateTime(DateTime.Today.AddHours(2)), 12, material.Id);
            _context.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();

            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 12);
            _context.Add(reservationMaterialCart);
            _context.SaveChanges();

            MaterialComment materialComment = new MaterialComment(reservationMaterialCart.Id, "Test comment", 5);
            // Arrange
            _context.MaterialComments.Add(materialComment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Create(materialComment.Id);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Get_ReturnsViewResult_WithoutMaterialComments()
        {
            MaterialComment materialComment = new MaterialComment(1, "Test comment", 5);
            
            // Act
            var result = await _controller.Create(materialComment.Id);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Get_ReturnsViewResult()
        {
            City city = new City("city");
            _context.Add(city);
            MaterialType materialType = new MaterialType("material type");
            _context.Add(materialType);
            MaterialStatus materialStatus = new MaterialStatus("material status");
            _context.Add(materialStatus);
            _context.SaveChanges();

            Material material = new Material("material", "description", 12, 12.12m, "12", city.Id, materialType.Id, materialStatus.Id);
            _context.Add(material);

            User user = new User();
            _context.Add(user);

            _context.SaveChanges();

            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12, false);
            _context.Add(materialReservation);

            ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), TimeOnly.FromDateTime(DateTime.Today.AddHours(1)), TimeOnly.FromDateTime(DateTime.Today.AddHours(2)), 12, material.Id);
            _context.Add(reservationTimeRangeMaterial);
            _context.SaveChanges();

            ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(material.Id, materialReservation.Id, user.Id, reservationTimeRangeMaterial.Id, 12);
            _context.Add(reservationMaterialCart);
            _context.SaveChanges();

            MaterialComment materialComment = new MaterialComment(reservationMaterialCart.Id, "Test comment", 5);
            // Arrange
            _context.MaterialComments.Add(materialComment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Create(reservationMaterialCart.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public async Task Create_Post_ExistingComment_ReturnsViewWithError()
        {
            // Arrange
            var user = new User();
            _context.Users.Add(user);

            var material = new Material("Boots", "Desc", 1, 1, "img", 1, 1, 1);
            _context.Materials.Add(material);

            var range = new ReservationTimeRangeMaterial(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(8)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(16)),
                10,
                material.Id
            );
            _context.ReservationTimeRangeMaterials.Add(range);
            await _context.SaveChangesAsync();

            var reservation = new ReservationMaterialCart(material.Id, 1, user.Id, range.Id, 10);
            _context.ReservationMaterialCarts.Add(reservation);
            await _context.SaveChangesAsync();

            // Comentario existente
            var existingComment = new MaterialComment(reservation.Id, "Old comment", 3);
            _context.MaterialComments.Add(existingComment);
            await _context.SaveChangesAsync();

            // Intento de crear un nuevo comentario para la misma reserva
            var viewModel = new MaterialCommentViewModel
            {
                ReservationMaterialCartId = reservation.Id,
                Text = "Updated comment",
                Raiting = 4
            };

            // Act
            var result = await _controller.Create(reservation.Id, viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MaterialCommentViewModel>(viewResult.Model);

            Assert.Equal("Updated comment", model.Text);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
            Assert.Contains("A comment already exists", _controller.ModelState[string.Empty].Errors.First().ErrorMessage);
        }


        [Fact]
        public async Task Create_Post_AddsComment_AndRedirects()
        {
            // Arrange
            var user = new User();
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            City city = new City("city");
            MaterialType materialType = new MaterialType("name");
            MaterialStatus materialStatus = new MaterialStatus("name");
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();

            var material = new Material("Ski", "Desc", 1, 1, "img", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);

            await _context.SaveChangesAsync();

            var timeRange = new ReservationTimeRangeMaterial(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(9)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(17)),
                10,
                material.Id
            );
            _context.ReservationTimeRangeMaterials.Add(timeRange);

            await _context.SaveChangesAsync();

            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12.12m, false);

            var reservationCart = new ReservationMaterialCart(material.Id, 1, user.Id, timeRange.Id, 10);
            _context.ReservationMaterialCarts.Add(reservationCart);
            await _context.SaveChangesAsync();

            var viewModel = new MaterialCommentViewModel
            {
                ReservationMaterialCartId = reservationCart.Id,
                Text = "Nice ski",
                Raiting = 5
            };

            // Act
            var result = await _controller.Create(reservationCart.Id, viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Post_AddsComment()
        {
            // Arrange
            var user = new User();
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            City city = new City("city");
            MaterialType materialType = new MaterialType("name");
            MaterialStatus materialStatus = new MaterialStatus("name");
            _context.Add(city);
            _context.Add(materialType);
            _context.Add(materialStatus);
            _context.SaveChanges();

            var material = new Material("Ski", "Desc", 1, 1, "img", city.Id, materialType.Id, materialStatus.Id);
            _context.Materials.Add(material);

            await _context.SaveChangesAsync();

            var timeRange = new ReservationTimeRangeMaterial(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(9)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(17)),
                10,
                material.Id
            );
            _context.ReservationTimeRangeMaterials.Add(timeRange);

            await _context.SaveChangesAsync();

            MaterialReservation materialReservation = new MaterialReservation(user.Id, 12.12m, false);
            _context.Add(materialReservation);
            _context.SaveChanges();

            var reservationCart = new ReservationMaterialCart(material.Id, 1, user.Id, timeRange.Id, 10);
            _context.ReservationMaterialCarts.Add(reservationCart);
            await _context.SaveChangesAsync();

            var viewModel = new MaterialCommentViewModel
            {
                ReservationMaterialCartId = reservationCart.Id,
                Text = "Nice ski",
                Raiting = 5
            };

            // Act
            var result = await _controller.Create(reservationCart.Id, viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_Get_UpdatesComment_AndRedirects()
        {
            // Arrange
            var user = new User();
            _context.Users.Add(user);

            var material = new Material("Boots", "Desc", 1, 1, "img", 1, 1, 1);
            _context.Materials.Add(material);

            var range = new ReservationTimeRangeMaterial(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(8)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(16)),
                10,
                material.Id
            );
            _context.ReservationTimeRangeMaterials.Add(range);
            await _context.SaveChangesAsync();

            var reservation = new ReservationMaterialCart(material.Id, 1, user.Id, range.Id, 10);
            _context.ReservationMaterialCarts.Add(reservation);
            await _context.SaveChangesAsync();

            var comment = new MaterialComment(reservation.Id, "Old comment", 2);
            _context.MaterialComments.Add(comment);
            await _context.SaveChangesAsync();

            var updatedViewModel = new MaterialCommentViewModel
            {
                Id = comment.Id,
                ReservationMaterialCartId = reservation.Id,
                Text = "Updated comment",
                Raiting = 4
            };

            // Act
            var result = await _controller.Edit(comment.Id);

            // Assert
            var redirectResult = Assert.IsType<ViewResult>(result); 

            var updatedComment = await _context.MaterialComments.FindAsync(comment.Id); 
        }

        [Fact]
        public async Task Edit_Get_UpdatesComment_AndRedirects_Id_Null()
        {
            // Arrange
            var user = new User();
            _context.Users.Add(user);

            var material = new Material("Boots", "Desc", 1, 1, "img", 1, 1, 1);
            _context.Materials.Add(material);

            var range = new ReservationTimeRangeMaterial(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(8)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(16)),
                10,
                material.Id
            );
            _context.ReservationTimeRangeMaterials.Add(range);
            await _context.SaveChangesAsync();

            var reservation = new ReservationMaterialCart(material.Id, 1, user.Id, range.Id, 10);
            _context.ReservationMaterialCarts.Add(reservation);
            await _context.SaveChangesAsync();

            var comment = new MaterialComment(reservation.Id, "Old comment", 2);
            _context.MaterialComments.Add(comment);
            await _context.SaveChangesAsync();

            var updatedViewModel = new MaterialCommentViewModel
            {
                Id = comment.Id,
                ReservationMaterialCartId = reservation.Id,
                Text = "Updated comment",
                Raiting = 4
            };

            // Act
            var result = await _controller.Edit(null);

            // Assert
            var redirectResult = Assert.IsType<NotFoundResult>(result);

            var updatedComment = await _context.MaterialComments.FindAsync(comment.Id);
        }

        [Fact]
        public async Task Edit_Get_UpdatesComment_AndRedirects_MaterialComment_NotFound()
        {
            // Arrange
            var user = new User();
            _context.Users.Add(user);

            var material = new Material("Boots", "Desc", 1, 1, "img", 1, 1, 1);
            _context.Materials.Add(material);

            var range = new ReservationTimeRangeMaterial(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(8)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(16)),
                10,
                material.Id
            );
            _context.ReservationTimeRangeMaterials.Add(range);
            await _context.SaveChangesAsync();

            var reservation = new ReservationMaterialCart(material.Id, 1, user.Id, range.Id, 10);
            _context.ReservationMaterialCarts.Add(reservation);
            await _context.SaveChangesAsync();

            var comment = new MaterialComment(reservation.Id, "Old comment", 2);
            
            var updatedViewModel = new MaterialCommentViewModel
            {
                Id = comment.Id,
                ReservationMaterialCartId = reservation.Id,
                Text = "Updated comment",
                Raiting = 4
            };

            // Act
            var result = await _controller.Edit(comment.Id);

            // Assert
            var redirectResult = Assert.IsType<NotFoundResult>(result);

            var updatedComment = await _context.MaterialComments.FindAsync(comment.Id);
        }

        [Fact]
        public async Task Edit_Post_UpdatesComment_AndRedirects()
        {
            // Arrange
            var user = new User();
            _context.Users.Add(user);

            var material = new Material("Boots", "Desc", 1, 1, "img", 1, 1, 1);
            _context.Materials.Add(material);

            var range = new ReservationTimeRangeMaterial(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(8)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(16)),
                10,
                material.Id
            );
            _context.ReservationTimeRangeMaterials.Add(range);
            await _context.SaveChangesAsync();

            var reservation = new ReservationMaterialCart(material.Id, 1, user.Id, range.Id, 10);
            _context.ReservationMaterialCarts.Add(reservation);
            await _context.SaveChangesAsync();

            var comment = new MaterialComment(reservation.Id, "Old comment", 2);
            _context.MaterialComments.Add(comment);
            await _context.SaveChangesAsync();

            var updatedViewModel = new MaterialCommentViewModel
            {
                Id = comment.Id,
                ReservationMaterialCartId = reservation.Id,
                Text = "Updated comment",
                Raiting = 4
            };

            // Act
            var result = await _controller.Edit(comment.Id, updatedViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("IndexUser", redirectResult.ActionName);

            var updatedComment = await _context.MaterialComments.FindAsync(comment.Id);
            Assert.Equal("Updated comment", updatedComment.Text);
        }

        [Fact]
        public async Task Edit_Post_UpdatesComment_AndRedirects_IdNotFound()
        {
            // Arrange
            var user = new User();
            _context.Users.Add(user);

            var material = new Material("Boots", "Desc", 1, 1, "img", 1, 1, 1);
            _context.Materials.Add(material);

            var range = new ReservationTimeRangeMaterial(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(8)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(16)),
                10,
                material.Id
            );
            _context.ReservationTimeRangeMaterials.Add(range);
            await _context.SaveChangesAsync();

            var reservation = new ReservationMaterialCart(material.Id, 1, user.Id, range.Id, 10);
            _context.ReservationMaterialCarts.Add(reservation);
            await _context.SaveChangesAsync();

            var comment = new MaterialComment(reservation.Id, "Old comment", 2);
            _context.MaterialComments.Add(comment);
            await _context.SaveChangesAsync();

            var updatedViewModel = new MaterialCommentViewModel
            {
                Id = 100,
                ReservationMaterialCartId = reservation.Id,
                Text = "Updated comment",
                Raiting = 4
            };

            // Act
            var result = await _controller.Edit(comment.Id, updatedViewModel);

            // Assert
            var redirectResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_UpdatesComment_AndRedirects_MaterialComment_NotFound()
        {
            // Arrange
            var user = new User();
            _context.Users.Add(user);

            var material = new Material("Boots", "Desc", 1, 1, "img", 1, 1, 1);
            _context.Materials.Add(material);

            var range = new ReservationTimeRangeMaterial(
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(8)),
                TimeOnly.FromDateTime(DateTime.Today.AddHours(16)),
                10,
                material.Id
            );
            _context.ReservationTimeRangeMaterials.Add(range);
            await _context.SaveChangesAsync();

            var reservation = new ReservationMaterialCart(material.Id, 1, user.Id, range.Id, 10);
            _context.ReservationMaterialCarts.Add(reservation);
            await _context.SaveChangesAsync();

            var comment = new MaterialComment(reservation.Id, "Old comment", 2);
            _context.MaterialComments.Add(comment);
            await _context.SaveChangesAsync();

            var updatedViewModel = new MaterialCommentViewModel
            {
                Id = 100,
                ReservationMaterialCartId = reservation.Id,
                Text = "Updated comment",
                Raiting = 4
            };

            // Act
            var result = await _controller.Edit(100, updatedViewModel);

            // Assert
            var redirectResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_RemovesComment_AndRedirects()
        {
            // Arrange
            var comment = new MaterialComment(4, "To be deleted", 3);
            _context.MaterialComments.Add(comment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(comment.Id);

            // Assert
            var redirectResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_RemovesComment_AndRedirects_Id_Null()
        {
            // Arrange
            var comment = new MaterialComment(4, "To be deleted", 3);
            _context.MaterialComments.Add(comment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(null);

            // Assert
            var redirectResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_RemovesComment_AndRedirects_MaterialComment_Null()
        {
            // Arrange
            var comment = new MaterialComment(4, "To be deleted", 3);

            // Act
            var result = await _controller.Delete(comment.Id);

            // Assert
            var redirectResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesComment_AndRedirects()
        {
            // Arrange
            var comment = new MaterialComment(4, "To be deleted", 3);
            _context.MaterialComments.Add(comment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteConfirmed(comment.Id);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("IndexUser", redirectResult.ActionName);
            Assert.DoesNotContain(_context.MaterialComments, c => c.Id == comment.Id);
        }
    }
}
