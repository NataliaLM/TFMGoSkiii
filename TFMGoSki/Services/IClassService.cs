using Microsoft.AspNetCore.Mvc.Rendering;
using TFMGoSki.Dtos;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public interface IClassService
    {
        Task<List<ClassDto>> GetAllClassesAsync();
        Task<ClassDto?> GetClassDetailsAsync(int id);
        Task CreateClassAsync(ClassViewModel model);
        Task<ClassViewModel?> GetEditViewModelAsync(int id);
        Task<bool> UpdateClassAsync(int id, ClassViewModel model);
        Task<bool> DeleteClassAsync(int id);
        SelectList GetInstructorsSelectList();
        SelectList GetClassLevelSelectList();
        SelectList GetCitiesSelectList();
    }
}
