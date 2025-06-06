﻿using Microsoft.AspNetCore.Hosting;
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
    public class ReservationTimeRangeClassesControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TFMGoSkiDbContext _context;

        public ReservationTimeRangeClassesControllerTest(WebApplicationFactory<Program> factory)
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
        public async Task Test_Reservation_Index_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/ReservationTimeRangeClasses");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Details()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            var @class = new Class("Name class", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2025, 06, 21), 
                new DateOnly(2025, 07, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 25, @class.Id);
            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            _context.SaveChanges();
            
            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Details/{reservationTimeRangeClass.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Details_ReturnsNotFound_ForInvalidId()
        {
            var response = await _client.GetAsync("/ReservationTimeRangeClasses/Details/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Details_ReturnsNotFound_IdNull()
        {
            var response = await _client.GetAsync("/ReservationTimeRangeClasses/Details?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Get_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/ReservationTimeRangeClasses/Create");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Post_RedirectsToIndex()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                ["Class"] = @class.Id.ToString(),
                ["StartDateOnly"] = "2025-08-21",
                ["EndDateOnly"] = "2025-09-22",
                ["StartTimeOnly"] = "11:25:46",
                ["EndTimeOnly"] = "12:26:47"
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Create_Post_InvalidModel()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            var viewModel = new ReservationTimeRangeClassViewModel
            {
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2025, 10, 21), new DateOnly(2025, 11, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Edit/{reservationTimeRangeClass.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Get_NotFound_Zero()
        {
            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Edit?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Get_NotFound_Id()
        {
            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Edit/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Post_Redirects()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2025, 12, 21), new DateOnly(2026, 01, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                ["Class"] = @class.Id.ToString(),
                ["StartDateOnly"] = "2025-08-21",
                ["EndDateOnly"] = "2025-09-22",
                ["StartTimeOnly"] = "11:25:46",
                ["EndTimeOnly"] = "12:26:47"
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Edit/{reservationTimeRangeClass.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Post_InvalidModel()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2026, 04, 21), new DateOnly(2026, 05, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var viewModel = new ReservationTimeRangeClassViewModel
            {
            };

            var json = JsonSerializer.Serialize(viewModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Edit/{reservationTimeRangeClass.Id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Edit_Post_NotFound_Id()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(
                new DateOnly(2026, 06, 21),
                new DateOnly(2026, 07, 22),
                new TimeOnly(11, 25, 46),
                new TimeOnly(12, 26, 47),
                15,
                @class.Id
            );

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var formData = new Dictionary<string, string>
            {
                { "Class", @class.Id.ToString() },
                { "StartDateOnly", "2026-08-21" },
                { "EndDateOnly", "2026-09-22" },
                { "StartTimeOnly", "11:25:46" },
                { "EndTimeOnly", "12:26:47" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/ReservationTimeRangeClasses/Edit/999", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Get_ReturnsSuccess()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2026, 10, 21), new DateOnly(2026, 11, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Delete/{reservationTimeRangeClass.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Get_NotFound_Zero()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2026, 12, 21), new DateOnly(2027, 01, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Delete?");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Get_NotFound()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2027, 02, 21), new DateOnly(2027, 03, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.GetAsync($"/ReservationTimeRangeClasses/Delete/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Post_Redirects()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2027, 04, 21), new DateOnly(2027, 05, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Delete/{reservationTimeRangeClass.Id}", new StringContent(""));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Reservation_Delete_Post_NotFound()
        {
            Instructor instructor = new Instructor("Name Instructor");
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            City city = new City("City Name");
            _context.Cities.Add(city);
            _context.SaveChanges();

            Class @class = new Class("Class Name", 150, 15, ClassLevel.Advanced, instructor.Id, city.Id);
            _context.Classes.Add(@class);
            _context.SaveChanges();

            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(new DateOnly(2027, 06, 21), new DateOnly(2027, 07, 22), new TimeOnly(11, 25, 46), new TimeOnly(12, 26, 47), 15, @class.Id);

            _context.ReservationTimeRangeClasses.Add(reservationTimeRangeClass);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsync($"/ReservationTimeRangeClasses/Delete/999", new StringContent(""));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
