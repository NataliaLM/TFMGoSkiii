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
    public class ReservationMaterialCartsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public ReservationMaterialCartsController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: ReservationMaterialCarts
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReservationMaterialCarts.ToListAsync());
        }

        // GET: ReservationMaterialCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationMaterialCart = await _context.ReservationMaterialCarts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationMaterialCart == null)
            {
                return NotFound();
            }

            return View(reservationMaterialCart);
        }

        // GET: ReservationMaterialCarts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReservationMaterialCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaterialId,MaterialReservationId,UserId,ReservationTimeRangeMaterialId,NumberMaterialsBooked")] ReservationMaterialCart reservationMaterialCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservationMaterialCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservationMaterialCart);
        }

        // GET: ReservationMaterialCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationMaterialCart = await _context.ReservationMaterialCarts.FindAsync(id);
            if (reservationMaterialCart == null)
            {
                return NotFound();
            }
            return View(reservationMaterialCart);
        }

        // POST: ReservationMaterialCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaterialId,MaterialReservationId,UserId,ReservationTimeRangeMaterialId,NumberMaterialsBooked")] ReservationMaterialCart reservationMaterialCart)
        {
            if (id != reservationMaterialCart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservationMaterialCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationMaterialCartExists(reservationMaterialCart.Id))
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
            return View(reservationMaterialCart);
        }

        // GET: ReservationMaterialCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationMaterialCart = await _context.ReservationMaterialCarts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationMaterialCart == null)
            {
                return NotFound();
            }

            return View(reservationMaterialCart);
        }

        // POST: ReservationMaterialCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservationMaterialCart = await _context.ReservationMaterialCarts.FindAsync(id);
            if (reservationMaterialCart != null)
            {
                _context.ReservationMaterialCarts.Remove(reservationMaterialCart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationMaterialCartExists(int id)
        {
            return _context.ReservationMaterialCarts.Any(e => e.Id == id);
        }
    }
}
