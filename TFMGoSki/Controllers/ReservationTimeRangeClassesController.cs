using Microsoft.AspNetCore.Mvc;
using TFMGoSki.Services;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class ReservationTimeRangeClassesController : Controller
    {
        private readonly IReservationTimeRangeClassService _service;

        public ReservationTimeRangeClassesController(IReservationTimeRangeClassService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _service.GetAllAsync();
            return View(reservations);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _service.GetByIdAsync(id.Value);
            return reservation == null ? NotFound() : View(reservation);
        }

        public async Task<IActionResult> Create()
        {
            // Ahora el servicio maneja el SelectList
            ViewBag.ClassId = await _service.GetClassSelectListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationTimeRangeClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ClassId = await _service.GetClassSelectListAsync();
                return View(model);
            }
            
            await _service.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var model = await _service.GetEditViewModelAsync(id.Value);
            if (model == null) return NotFound();

            // El SelectList es manejado por el servicio
            ViewBag.ClassId = await _service.GetClassSelectListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] ReservationTimeRangeClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ClassId = await _service.GetClassSelectListAsync();
                return View(model);
            }

            var updated = await _service.UpdateAsync(id, model);
            return updated ? RedirectToAction(nameof(Index)) : NotFound();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _service.GetByIdAsync(id.Value);
            return reservation == null ? NotFound() : View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? RedirectToAction(nameof(Index)) : NotFound();
        }
    }
}
