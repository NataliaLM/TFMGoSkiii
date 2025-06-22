using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Models;

namespace TFMGoSki.Controllers
{
    public class MaterialStatusController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public MaterialStatusController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: MaterialStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaterialStatuses.ToListAsync());
        }

        // GET: MaterialStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialStatus = await _context.MaterialStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialStatus == null)
            {
                return NotFound();
            }

            return View(materialStatus);
        }

        // GET: MaterialStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaterialStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            MaterialStatus materialStatus = new MaterialStatus(name);
            if (ModelState.IsValid)
            {
                _context.Add(materialStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(materialStatus);
        }

        // GET: MaterialStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialStatus = await _context.MaterialStatuses.FindAsync(id);
            if (materialStatus == null)
            {
                return NotFound();
            }
            return View(materialStatus);
        }

        // POST: MaterialStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] MaterialStatus materialStatus)
        {
            if (id != materialStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materialStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialStatusExists(materialStatus.Id))
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
            return View(materialStatus);
        }

        // GET: MaterialStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialStatus = await _context.MaterialStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialStatus == null)
            {
                return NotFound();
            }

            return View(materialStatus);
        }

        // POST: MaterialStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materialStatus = await _context.MaterialStatuses.FindAsync(id);
            if (materialStatus != null)
            {
                _context.MaterialStatuses.Remove(materialStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialStatusExists(int id)
        {
            return _context.MaterialStatuses.Any(e => e.Id == id);
        }
    }
}
