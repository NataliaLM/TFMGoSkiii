using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Services;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    [Authorize(Roles = "Admin,Worker")]
    public class ReservationTimeRangeClassesController : Controller
    {
        private readonly IReservationTimeRangeClassService _service;
        private readonly TFMGoSkiDbContext _context;

        public ReservationTimeRangeClassesController(IReservationTimeRangeClassService service, TFMGoSkiDbContext context)
        {
            _service = service;
            _context = context;
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

        public async Task<IActionResult> Create(int? classId)
        {
            var model = new ReservationTimeRangeClassViewModel();

            if (classId.HasValue)
            {
                model.Class = classId.Value;
                ViewBag.FixedClass = true; // Indicador para la vista
            }
            else
            {
                ViewBag.ClassId = await _service.GetClassSelectListAsync();
                ViewBag.FixedClass = false;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReservationTimeRangeClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (model.Class != 0)
                {
                    ViewBag.FixedClass = true;
                }
                else
                {
                    ViewBag.ClassId = await _service.GetClassSelectListAsync();
                    ViewBag.FixedClass = false;
                }

                return View(model);
            }

            var result = await _service.CreateAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                if (model.Class != 0)
                {
                    ViewBag.FixedClass = true;
                }
                else
                {
                    ViewBag.ClassId = await _service.GetClassSelectListAsync();
                    ViewBag.FixedClass = false;
                }
                return View(model);
            }

            if(model.Class != 0)
            {
                return RedirectToAction("Details", "Classes", new { id = model.Class });
            }

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
        public async Task<IActionResult> Edit(int id, ReservationTimeRangeClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ClassId = await _service.GetClassSelectListAsync();
                return View(model);
            }

            var result = await _service.UpdateAsync(id, model);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Error al actualizar la reserva.");
                ViewBag.ClassId = await _service.GetClassSelectListAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
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
            var reservationTimeRange = _context.ClassReservations
                                       .Where(r => r.ReservationTimeRangeClassId == id)
                                       .ToList(); // Ejecuta la consulta

            if (reservationTimeRange.Any())
            {
                // Recupera el modelo nuevamente
                var reservationTimeRangeClassDto = await _service.GetByIdAsync(id);

                // Agrega el error al modelo
                ModelState.AddModelError(string.Empty, "The reservation time range class cannot be deleted because it has associated class reservations.");

                // Devuelve la vista Delete con el modelo y el error
                return View("Delete", reservationTimeRangeClassDto);
            }

            var deleted = await _service.DeleteAsync(id);
            return deleted ? RedirectToAction(nameof(Index)) : NotFound();
        }
    }
}
