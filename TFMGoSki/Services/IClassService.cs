using Microsoft.AspNetCore.Mvc.Rendering;
using TFMGoSki.Dtos;
using TFMGoSki.Exceptions;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public interface IClassService
    {
        Task<List<ClassDto>> GetAllClassesAsync(bool? finalizadas = null);
        Task<ClassDto?> GetClassDetailsAsync(int id);
        Task<bool> CreateClassAsync(ClassViewModel model);
        Task<ClassViewModel?> GetEditViewModelAsync(int id);
        Task<UpdateResult> UpdateClassAsync(int id, ClassViewModel model);
        Task<bool> DeleteClassAsync(int id);
        SelectList GetInstructorsSelectList();
        SelectList GetClassLevelSelectList();
        SelectList GetCitiesSelectList();
    }
}
