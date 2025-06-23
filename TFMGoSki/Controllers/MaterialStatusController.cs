using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

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
        [HttpPost] 
        public async Task<IActionResult> Create(MaterialStatusViewModel materialStatusViewModel)
        {
            var materialStatusFound = _context.MaterialStatuses.FirstOrDefault(c => c.Name.Equals(materialStatusViewModel.Name));

            if (materialStatusFound != null)
            {
                ModelState.AddModelError("Name", "There is already a material status with this name.");
                return View(materialStatusViewModel);
            }

            MaterialStatus materialStatus = new MaterialStatus(materialStatusViewModel.Name);
            if (ModelState.IsValid)
            {
                _context.Add(materialStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(materialStatusViewModel);
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

            MaterialStatusViewModel materialStatusViewModel = new MaterialStatusViewModel()
            {
                Id = materialStatus.Id,
                Name = materialStatus.Name
            };

            return View(materialStatusViewModel);
        }

        // POST: MaterialStatus/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, MaterialStatusViewModel materialStatusViewModel)
        {
            if (id != materialStatusViewModel.Id)
            {
                return NotFound();
            }

            var materialStatusFound = _context.MaterialTypes.FirstOrDefault(c => c.Name.Equals(materialStatusViewModel.Name) && c.Id != id);

            if (materialStatusFound != null)
            {
                ModelState.AddModelError("Name", "There is already a material status with this name.");
                return View(materialStatusViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    MaterialStatus? materialStatus = _context.MaterialStatuses.FirstOrDefault(x => x.Id == id);
                    materialStatus.Update(materialStatusViewModel.Name);
                    _context.Update(materialStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialStatusExists(materialStatusViewModel.Id))
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
            return View(materialStatusViewModel);
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
