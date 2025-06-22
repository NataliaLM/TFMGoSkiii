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
    public class MaterialReservationsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public MaterialReservationsController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: MaterialReservations
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaterialReservations.ToListAsync());
        }

        // GET: MaterialReservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialReservation = await _context.MaterialReservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialReservation == null)
            {
                return NotFound();
            }

            return View(materialReservation);
        }

        // GET: MaterialReservations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaterialReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Total")] MaterialReservation materialReservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(materialReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(materialReservation);
        }

        // GET: MaterialReservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialReservation = await _context.MaterialReservations.FindAsync(id);
            if (materialReservation == null)
            {
                return NotFound();
            }
            return View(materialReservation);
        }

        // POST: MaterialReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Total")] MaterialReservation materialReservation)
        {
            if (id != materialReservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materialReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialReservationExists(materialReservation.Id))
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
            return View(materialReservation);
        }

        // GET: MaterialReservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialReservation = await _context.MaterialReservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialReservation == null)
            {
                return NotFound();
            }

            return View(materialReservation);
        }

        // POST: MaterialReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materialReservation = await _context.MaterialReservations.FindAsync(id);
            if (materialReservation != null)
            {
                _context.MaterialReservations.Remove(materialReservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialReservationExists(int id)
        {
            return _context.MaterialReservations.Any(e => e.Id == id);
        }
    }
}
