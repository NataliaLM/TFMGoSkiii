using Microsoft.AspNetCore.Mvc;
using TFMGoSki.Services;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IInstructorService _instructorService;

        public InstructorsController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
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

            await _instructorService.CreateAsync(viewModel);
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
            return updated ? RedirectToAction(nameof(Index)) : NotFound();
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
            var deleted = await _instructorService.DeleteAsync(id);
            return deleted ? RedirectToAction(nameof(Index)) : NotFound();
        }
    }
}
