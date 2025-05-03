using TFMGoSki.Dtos;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public interface ICityService
    {
        Task<List<CityDto>> GetAllAsync();
        Task<CityDto?> GetByIdAsync(int id);
        Task<City?> FindEntityByIdAsync(int id);
        Task CreateAsync(CityViewModel cityViewModel);
        Task<bool> UpdateAsync(int id, CityViewModel viewModel);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
