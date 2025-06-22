using Microsoft.AspNetCore.Identity;
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
    public class MaterialReservationsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;
        private readonly UserManager<User> _userManager;

        public MaterialReservationsController(TFMGoSkiDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MaterialReservations
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaterialReservations.ToListAsync());
        }
        public async Task<IActionResult> IndexUser()
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

        [HttpPost]
        public async Task<IActionResult> Create(MaterialReservationViewModel materialReservationViewModel)
        {
            var userId = _userManager.GetUserId(User);
            
            MaterialReservation materialReservation = new MaterialReservation(int.Parse(userId), 0, false);
            if (ModelState.IsValid)
            {
                _context.Add(materialReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexUser));
            }
            return View(materialReservationViewModel);
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
