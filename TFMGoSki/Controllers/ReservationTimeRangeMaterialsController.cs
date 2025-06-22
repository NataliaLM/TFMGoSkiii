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
    public class ReservationTimeRangeMaterialsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public ReservationTimeRangeMaterialsController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: ReservationTimeRangeMaterials
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReservationTimeRangeMaterials.ToListAsync());
        }

        // GET: ReservationTimeRangeMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationTimeRangeMaterial = await _context.ReservationTimeRangeMaterials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationTimeRangeMaterial == null)
            {
                return NotFound();
            }

            return View(reservationTimeRangeMaterial);
        }

        // GET: ReservationTimeRangeMaterials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReservationTimeRangeMaterials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RemainingMaterialsQuantity,MaterialId,Id,StartDateOnly,EndDateOnly,StartTimeOnly,EndTimeOnly")] ReservationTimeRangeMaterial reservationTimeRangeMaterial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservationTimeRangeMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservationTimeRangeMaterial);
        }

        // GET: ReservationTimeRangeMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationTimeRangeMaterial = await _context.ReservationTimeRangeMaterials.FindAsync(id);
            if (reservationTimeRangeMaterial == null)
            {
                return NotFound();
            }
            return View(reservationTimeRangeMaterial);
        }

        // POST: ReservationTimeRangeMaterials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RemainingMaterialsQuantity,MaterialId,Id,StartDateOnly,EndDateOnly,StartTimeOnly,EndTimeOnly")] ReservationTimeRangeMaterial reservationTimeRangeMaterial)
        {
            if (id != reservationTimeRangeMaterial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservationTimeRangeMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationTimeRangeMaterialExists(reservationTimeRangeMaterial.Id))
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
            return View(reservationTimeRangeMaterial);
        }

        // GET: ReservationTimeRangeMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationTimeRangeMaterial = await _context.ReservationTimeRangeMaterials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationTimeRangeMaterial == null)
            {
                return NotFound();
            }

            return View(reservationTimeRangeMaterial);
        }

        // POST: ReservationTimeRangeMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservationTimeRangeMaterial = await _context.ReservationTimeRangeMaterials.FindAsync(id);
            if (reservationTimeRangeMaterial != null)
            {
                _context.ReservationTimeRangeMaterials.Remove(reservationTimeRangeMaterial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationTimeRangeMaterialExists(int id)
        {
            return _context.ReservationTimeRangeMaterials.Any(e => e.Id == id);
        }
    }
}
