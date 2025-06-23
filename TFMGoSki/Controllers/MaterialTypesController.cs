using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFMGoSki.Data;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class MaterialTypesController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public MaterialTypesController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: MaterialTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaterialTypes.ToListAsync());
        }

        // GET: MaterialTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialType = await _context.MaterialTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialType == null)
            {
                return NotFound();
            }

            return View(materialType);
        }

        // GET: MaterialTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaterialTypes/Create
        [HttpPost]
        public async Task<IActionResult> Create(MaterialTypeViewModel materialTypeViewModel)
        {
            var materialTypeFound = _context.MaterialTypes.FirstOrDefault(c => c.Name.Equals(materialTypeViewModel.Name));

            if (materialTypeFound != null)
            {
                ModelState.AddModelError("Name", "There is already a material type with this name.");
                return View(materialTypeViewModel);
            }

            if (ModelState.IsValid)
            {
                MaterialType materialType = new MaterialType(materialTypeViewModel.Name);
                _context.Add(materialType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(materialTypeViewModel);
        }

        // GET: MaterialTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialType = await _context.MaterialTypes.FindAsync(id);
            if (materialType == null)
            {
                return NotFound();
            }
            return View(materialType);
        }

        // POST: MaterialTypes/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, MaterialTypeViewModel materialTypeViewModel)
        {
            var materialTypeFound = _context.MaterialTypes.FirstOrDefault(c => c.Name.Equals(materialTypeViewModel.Name) && c.Id != id);

            if (materialTypeFound != null)
            {
                ModelState.AddModelError("Name", "There is already a material type with this name.");
                return View(materialTypeViewModel);
            }

            if (id != materialTypeViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    MaterialType? materialType = _context.MaterialTypes.FirstOrDefault(m => m.Id == materialTypeViewModel.Id);
                    if (materialType == null)
                    {
                        return NotFound();
                    }

                    materialType.Update(materialTypeViewModel.Name);
                    _context.Update(materialType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialTypeExists(materialTypeViewModel.Id))
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
            return View(materialTypeViewModel);
        }

        // GET: MaterialTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialType = await _context.MaterialTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialType == null)
            {
                return NotFound();
            }

            return View(materialType);
        }

        // POST: MaterialTypes/Delete/5
        [HttpPost, ActionName("Delete")] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materialType = await _context.MaterialTypes.FindAsync(id);
            if (materialType != null)
            {
                _context.MaterialTypes.Remove(materialType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialTypeExists(int id)
        {
            return _context.MaterialTypes.Any(e => e.Id == id);
        }
    }
}
