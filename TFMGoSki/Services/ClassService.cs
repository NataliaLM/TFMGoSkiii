using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public class ClassService : IClassService
    {
        private readonly TFMGoSkiDbContext _context;

        public ClassService(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClassDto>> GetAllClassesAsync()
        {
            var classes = await _context.Classes.ToListAsync();
            var classDtos = new List<ClassDto>();

            foreach (var clase in classes)
            {
                var city = await _context.Cities.FindAsync(clase.CityId);
                var instructor = await _context.Instructors.FindAsync(clase.InstructorId);

                classDtos.Add(new ClassDto
                {
                    Id = clase.Id,
                    Name = clase.Name,
                    Price = clase.Price,
                    StudentQuantity = clase.StudentQuantity,
                    ClassLevel = clase.ClassLevel,
                    InstructorName = instructor?.Name,
                    CityName = city?.Name
                });
            }

            return classDtos;
        }

        public async Task<ClassDto?> GetClassDetailsAsync(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return null;

            var city = await _context.Cities.FindAsync(@class.CityId);
            var instructor = await _context.Instructors.FindAsync(@class.InstructorId);

            return new ClassDto
            {
                Id = @class.Id,
                Name = @class.Name,
                Price = @class.Price,
                StudentQuantity = @class.StudentQuantity,
                ClassLevel = @class.ClassLevel,
                InstructorName = instructor?.Name,
                CityName = city?.Name
            };
        }

        public async Task CreateClassAsync(ClassViewModel model)
        {
            var @class = new Class(
                model.Name,
                model.Price!.Value,
                model.StudentQuantity!.Value,
                model.ClassLevel!.Value,
                model.Instructor!.Value,
                model.City!.Value
            );

            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();
        }

        public async Task<ClassViewModel?> GetEditViewModelAsync(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return null;

            return new ClassViewModel
            {
                Id = @class.Id,
                Name = @class.Name,
                Price = @class.Price,
                StudentQuantity = @class.StudentQuantity,
                ClassLevel = @class.ClassLevel,
                Instructor = @class.InstructorId,
                City = @class.CityId
            };
        }

        public async Task<bool> UpdateClassAsync(int id, ClassViewModel model)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return false;

            @class.Update(model.Name, model.Price!.Value, model.StudentQuantity!.Value,
                          model.ClassLevel!.Value, model.Instructor!.Value, model.City!.Value);

            _context.Classes.Update(@class);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClassAsync(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return false;

            _context.Classes.Remove(@class);
            await _context.SaveChangesAsync();
            return true;
        }

        public SelectList GetInstructorsSelectList() =>
            new SelectList(_context.Instructors, "Id", "Name");

        public SelectList GetClassLevelSelectList() =>
            new SelectList(Enum.GetValues(typeof(ClassLevel)));

        public SelectList GetCitiesSelectList() =>
            new SelectList(_context.Cities, "Id", "Name");
    }
}
