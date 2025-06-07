using TFMGoSki.Dtos;
using TFMGoSki.Exceptions;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public interface IInstructorService
    {
        Task<List<InstructorDto>> GetAllAsync();
        Task<InstructorDto?> GetDetailsAsync(int id);
        Task<bool> CreateAsync(InstructorViewModel viewModel);
        Task<InstructorViewModel?> GetEditViewModelAsync(int id);
        Task<UpdateResult> UpdateAsync(int id, InstructorViewModel viewModel);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);
    }
}
