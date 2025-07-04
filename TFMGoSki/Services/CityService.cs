﻿using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Exceptions;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public class CityService : ICityService
    {
        private readonly TFMGoSkiDbContext _context;

        public CityService(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        public async Task<List<CityDto>> GetAllAsync()
        {
            return await _context.Cities
                .Select(city => new CityDto
                {
                    Id = city.Id,
                    Name = city.Name
                }).ToListAsync();
        }

        public async Task<CityDto?> GetByIdAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            return city == null ? null : new CityDto { Id = city.Id, Name = city.Name };
        }

        public async Task<City?> FindEntityByIdAsync(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

        public async Task<bool> CreateAsync(CityViewModel cityViewModel)
        {
            var cityFound = _context.Cities.FirstOrDefault(c => c.Name.Equals(cityViewModel.Name));

            if (cityFound != null)
            {
                return false;
            }

            City city = new City(cityViewModel.Name);
            _context.Add(city);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UpdateResult> UpdateAsync(int id, CityViewModel viewModel)
        {
            var duplicate = _context.Cities
                .FirstOrDefault(c => c.Name.Equals(viewModel.Name) && c.Id != id);

            if (duplicate != null)
            {
                return UpdateResult.Fail("A city with this name already exists.");
            }

            City? city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return UpdateResult.Fail("City not found.");
            }
            city.Update(viewModel.Name);
            _context.Update(city);
            await _context.SaveChangesAsync();
            return UpdateResult.Ok();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null) return false;

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Cities.AnyAsync(e => e.Id == id);
        }
    }
}
