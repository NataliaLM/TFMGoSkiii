using Microsoft.AspNetCore.Mvc;
using TFMGoSki.Services;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IClassService _classService;

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        public async Task<IActionResult> Index(bool? finalizadas)
        {
            var classDtos = await _classService.GetAllClassesAsync(finalizadas);
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
                ViewBag.Instructor = _classService.GetInstructorsSelectList();
                ViewBag.ClassLevel = _classService.GetClassLevelSelectList();
                ViewBag.City = _classService.GetCitiesSelectList();
                return View(model);
            }            

            await _classService.CreateClassAsync(model);
            return RedirectToAction(nameof(Index));
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
                ViewBag.Instructor = _classService.GetInstructorsSelectList();
                ViewBag.ClassLevel = _classService.GetClassLevelSelectList();
                ViewBag.City = _classService.GetCitiesSelectList();
                return View(model);
            }            

            var updated = await _classService.UpdateClassAsync(id, model);
            return updated ? RedirectToAction(nameof(Index)) : NotFound();
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
