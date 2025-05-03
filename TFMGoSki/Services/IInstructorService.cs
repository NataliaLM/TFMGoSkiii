using TFMGoSki.Dtos;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public interface IInstructorService
    {
        Task<List<InstructorDto>> GetAllAsync();
        Task<InstructorDto?> GetDetailsAsync(int id);
        Task CreateAsync(InstructorViewModel viewModel);
        Task<InstructorViewModel?> GetEditViewModelAsync(int id);
        Task<bool> UpdateAsync(int id, InstructorViewModel viewModel);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);
    }
}
