using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Exceptions;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly TFMGoSkiDbContext _context;

        public InstructorService(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        public async Task<List<InstructorDto>> GetAllAsync()
        {
            var instructors = await _context.Instructors.ToListAsync();
            return instructors.Select(i => new InstructorDto
            {
                Id = i.Id,
                Name = i.Name
            }).ToList();
        }

        public async Task<InstructorDto?> GetDetailsAsync(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return null;

            return new InstructorDto
            {
                Id = instructor.Id,
                Name = instructor.Name
            };
        }

        public async Task<bool> CreateAsync(InstructorViewModel viewModel)
        {
            var instructorFound = _context.Instructors.FirstOrDefault(i => i.Name == viewModel.Name);
            if(instructorFound != null)
            {
                return false;
            }

            var instructor = new Instructor(viewModel.Name);
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<InstructorViewModel?> GetEditViewModelAsync(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return null;

            return new InstructorViewModel
            {
                Id = instructor.Id,
                Name = instructor.Name
            };
        }

        public async Task<UpdateResult> UpdateAsync(int id, InstructorViewModel viewModel)
        {            
            var duplicate = _context.Instructors
                .FirstOrDefault(c => c.Name.Equals(viewModel.Name) && c.Id != id);

            if (duplicate != null)
            {
                return UpdateResult.Fail("An instructor with that name already exists.");
            }

            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor == null)
            {
                return UpdateResult.Fail("Instructor not found.");
            }

            instructor.Update(viewModel.Name);
            _context.Update(instructor);
            await _context.SaveChangesAsync();
            return UpdateResult.Ok();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return false;

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Instructors.Any(e => e.Id == id);
        }
    }
}
