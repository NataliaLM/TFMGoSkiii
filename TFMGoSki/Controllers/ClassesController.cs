using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Models;

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
            return View(await _context.Classes.ToListAsync());
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

            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            ViewBag.InstructorId = new SelectList(_context.Instructors, "Id", "Name");
            ViewBag.ClassLevel = new SelectList(Enum.GetValues(typeof(ClassLevel)));
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,StudentQuantity,ClassLevel,InstructorId")] string name, decimal price, int studentQuantity, ClassLevel classLevel, int instructorId)
        {
            Class @class = new Class(name, price, studentQuantity, classLevel, instructorId);

            if (@class != null)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(@class);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(@class);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }

            ViewBag.InstructorId = new SelectList(_context.Instructors, "Id", "Name");
            ViewBag.ClassLevel = new SelectList(Enum.GetValues(typeof(ClassLevel)));

            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Price,StudentQuantity,ClassLevel,InstructorId")] string name, decimal price, int studentQuantity, ClassLevel classLevel, int instructorId)
        {
            Class? @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {                
                if (ModelState.IsValid)
                {
                    try
                    {
                        @class.Update(name, price, studentQuantity, classLevel, instructorId);

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
                return View(@class);
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

            return View(@class);
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
