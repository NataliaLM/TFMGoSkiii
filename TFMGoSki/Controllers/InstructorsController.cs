using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Services;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    [Authorize(Roles = "Admin,Worker")]
    public class InstructorsController : Controller
    {
        private readonly IInstructorService _instructorService;
        private readonly TFMGoSkiDbContext _context;

        public InstructorsController(IInstructorService instructorService, TFMGoSkiDbContext context)
        {
            _instructorService = instructorService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var instructors = await _instructorService.GetAllAsync();
            return View(instructors);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _instructorService.GetDetailsAsync(id.Value);
            return instructor == null ? NotFound() : View(instructor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(InstructorViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var created = await _instructorService.CreateAsync(viewModel);
            if(created == false)
            {
                return View(viewModel);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var viewModel = await _instructorService.GetEditViewModelAsync(id.Value);
            return viewModel == null ? NotFound() : View(viewModel);
        }

        [HttpPost] 
        public async Task<IActionResult> Edit(int id, InstructorViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var updated = await _instructorService.UpdateAsync(id, viewModel);

            if (!updated.Success)
            {
                ModelState.AddModelError("Name", updated.ErrorMessage ?? "Error updating the instructor.");
                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _instructorService.GetDetailsAsync(id.Value);
            return instructor == null ? NotFound() : View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classes = _context.Classes
                                .Where(i => i.InstructorId == id)
                                .ToList();

            if (classes.Any())
            {
                // Recupera el modelo nuevamente
                var instructorDto = await _instructorService.GetDetailsAsync(id);

                // Agrega el error al modelo
                ModelState.AddModelError(string.Empty, "The instructor cannot be deleted because it has associated classes.");

                // Devuelve la vista Delete con el modelo y el error
                return View("Delete", instructorDto);
            }
            var deleted = await _instructorService.DeleteAsync(id);
            return deleted ? RedirectToAction(nameof(Index)) : NotFound();
        }
    }
}
