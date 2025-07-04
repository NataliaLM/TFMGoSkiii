﻿using TFMGoSki.Dtos;
using TFMGoSki.Exceptions;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public interface ICityService
    {
        Task<List<CityDto>> GetAllAsync();
        Task<CityDto?> GetByIdAsync(int id);
        Task<City?> FindEntityByIdAsync(int id);
        Task<bool> CreateAsync(CityViewModel cityViewModel);
        Task<UpdateResult> UpdateAsync(int id, CityViewModel viewModel);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
