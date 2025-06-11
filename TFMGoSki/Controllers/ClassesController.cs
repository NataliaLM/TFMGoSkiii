using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Services;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    [Authorize(Roles = "Admin,Worker")]
    public class ClassesController : Controller
    {
        private readonly IClassService _classService;
        private readonly TFMGoSkiDbContext _context;
        public ClassesController(IClassService classService, TFMGoSkiDbContext context)
        {
            _classService = classService;
            _context = context;
        }

        public async Task<IActionResult> Index(bool? finalizadas)
        {
            var classDtos = await _classService.GetAllClassesAsync(finalizadas);
            return View(classDtos);
        }

        public async Task<IActionResult> IndexUser(bool? finalizadas, string name, decimal? minPrice, decimal? maxPrice, string classLevel, string cityName, DateOnly? minDate, DateOnly? maxDate)
        {
            // Obtener todos los niveles distintos disponibles en la base de datos
            var classLevels = await _context.Classes
                .Select(c => c.ClassLevel)
                .Distinct()
                .ToListAsync();

            ViewBag.ClassLevels = new SelectList(
                classLevels.Select(c => new { Value = c.ToString(), Text = c.ToString() }),
                "Value", "Text"
            );

            // Obtener todos los niveles distintos disponibles en la base de datos
            var cityNames = await _context.Cities
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();

            ViewBag.CityList = new SelectList(
                cityNames.Select(c => new { Value = c, Text = c }),
                "Value", "Text"
                ); // Carga el ViewBag con la lista para el dropdown

            var classDtos = await _classService.GetAllClassesUserAsync(finalizadas, name, minPrice, maxPrice, classLevel, cityName, minDate, maxDate);
            return View(classDtos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var classDto = await _classService.GetClassDetailsAsync(id.Value);
            return classDto == null ? NotFound() : View(classDto);
        }

        public IActionResult Create()
        {
            ViewBag.Instructor = _classService.GetInstructorsSelectList();
            ViewBag.ClassLevel = _classService.GetClassLevelSelectList();
            ViewBag.City = _classService.GetCitiesSelectList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return View(model);
            }

            var created = await _classService.CreateClassAsync(model);
            if (!created)
            {
                ModelState.AddModelError("Name", "A class with this name already exists.");
                LoadSelectLists();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        private void LoadSelectLists()
        {
            ViewBag.Instructor = _classService.GetInstructorsSelectList();
            ViewBag.ClassLevel = _classService.GetClassLevelSelectList();
            ViewBag.City = _classService.GetCitiesSelectList();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var model = await _classService.GetEditViewModelAsync(id.Value);
            if (model == null) return NotFound();

            ViewBag.Instructor = _classService.GetInstructorsSelectList();
            ViewBag.ClassLevel = _classService.GetClassLevelSelectList();
            ViewBag.City = _classService.GetCitiesSelectList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return View(model);
            }

            var result = await _classService.UpdateClassAsync(id, model);

            if (!result.Success)
            {
                ModelState.AddModelError("Name", result.ErrorMessage ?? "Error updating the class.");
                LoadSelectLists();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var classDto = await _classService.GetClassDetailsAsync(id.Value);
            return classDto == null ? NotFound() : View(classDto);
        }

        [HttpPost, ActionName("Delete")] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _classService.DeleteClassAsync(id);
            return deleted ? RedirectToAction(nameof(Index)) : NotFound();
        }
    }
}
