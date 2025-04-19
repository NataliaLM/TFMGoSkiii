using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public InstructorsController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: Instructors
        public async Task<IActionResult> Index()
        {
            List<Instructor>? instructors = await _context.Instructors.ToListAsync();
            List<InstructorDto> instructorDtos = new List<InstructorDto>();

            foreach (Instructor? instructor in instructors)
            {
                InstructorDto instructorDto = new InstructorDto
                {
                    Id = instructor.Id,
                    Name = instructor.Name
                };

                instructorDtos.Add(instructorDto);
            }

            return View(instructorDtos);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            InstructorDto instructorDto = new InstructorDto
            {
                Id = instructor.Id,
                Name = instructor.Name
            };

            return View(instructorDto);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstructorViewModel instructorViewModel)
        {
            Instructor instructor = new Instructor(instructorViewModel.Name);

            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructorViewModel);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            InstructorViewModel instructorViewModel = new InstructorViewModel
            {
                Id = instructor.Id,
                Name = instructor.Name
            };
            return View(instructorViewModel);
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InstructorViewModel instructorViewModel)
        {
            Instructor? instructor = _context.Instructors.FirstOrDefault(instructor => instructor.Id == id);

            if (instructor != null)
            {                
                if (ModelState.IsValid)
                {
                    try
                    {
                        instructor.Update(instructorViewModel.Name);

                        _context.Update(instructor);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!InstructorExists(instructor.Id))
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
            }
            return View(instructorViewModel);
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            InstructorDto instructorDto = new InstructorDto
            {
                Id = instructor.Id,
                Name = instructor.Name
            };

            return View(instructorDto);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.Id == id);
        }
    }
}
