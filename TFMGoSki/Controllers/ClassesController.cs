using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class ClassesController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public ClassesController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            List<Class>? classes = await _context.Classes.ToListAsync();
            List<ClassDto> classDtos = new List<ClassDto>();

            foreach (Class? clase in classes)
            {
                ClassDto classDto = new ClassDto
                {
                    Id = clase.Id,
                    Name = clase.Name,
                    Price = clase.Price,
                    StudentQuantity = clase.StudentQuantity,
                    ClassLevel = clase.ClassLevel
                };
                Instructor? instructor = await _context.Instructors.FindAsync(clase.InstructorId);
                classDto.InstructorName = instructor.Name; 

                classDtos.Add(classDto);
            }
            
            return View(classDtos);
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            ClassDto classDto = new ClassDto
            {
                Id = @class.Id,
                Name = @class.Name,
                Price = @class.Price,
                StudentQuantity = @class.StudentQuantity,
                ClassLevel = @class.ClassLevel
            };

            return View(classDto);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            ViewBag.Instructor = new SelectList(_context.Instructors, "Id", "Name");
            ViewBag.ClassLevel = new SelectList(Enum.GetValues(typeof(ClassLevel)));
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                Class? @class = new Class(
                    model.Name,
                    model.Price.Value,
                    model.StudentQuantity.Value,
                    model.ClassLevel.Value,
                    model.Instructor.Value
                );

                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Class? @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }

            ViewBag.Instructor = new SelectList(_context.Instructors, "Id", "Name");
            ViewBag.ClassLevel = new SelectList(Enum.GetValues(typeof(ClassLevel)));

            ClassViewModel model = new ClassViewModel
            {
                Id = @class.Id,
                Name = @class.Name,
                Price = @class.Price,
                StudentQuantity = @class.StudentQuantity,
                ClassLevel = @class.ClassLevel,
                Instructor = @class.InstructorId
            };

            return View(model);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassViewModel model)//TODO: (Natalia) ¿El Id ya está? Arriba lo uso.
        {
            Class? @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {                
                if (ModelState.IsValid)
                {
                    try
                    {
                        @class.Update(model.Name, model.Price.Value, model.StudentQuantity.Value, model.ClassLevel.Value, model.Instructor.Value);

                        _context.Update(@class);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ClassExists(@class.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            ClassDto classDto = new ClassDto
            {
                Id = @class.Id,
                Name = @class.Name,
                Price = @class.Price,
                StudentQuantity = @class.StudentQuantity,
                ClassLevel = @class.ClassLevel
            };

            return View(classDto);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }
    }
}
