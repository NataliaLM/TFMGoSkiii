using Microsoft.AspNetCore.Mvc;
using TFMGoSki.Dtos;
using TFMGoSki.Services;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class CitiesController : Controller
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        public async Task<IActionResult> Index()
        {
            var cities = await _cityService.GetAllAsync();
            return View(cities);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var city = await _cityService.GetByIdAsync(id.Value);
            if (city == null) return NotFound();

            return View(city);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CityViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            await _cityService.CreateAsync(viewModel);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var city = await _cityService.FindEntityByIdAsync(id.Value);
            if (city == null) return NotFound();

            return View(new CityViewModel { Id = city.Id, Name = city.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CityViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var updated = await _cityService.UpdateAsync(id, viewModel);
            if (!updated) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var city = await _cityService.GetByIdAsync(id.Value);
            if (city == null) return NotFound();

            return View(city);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _cityService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
