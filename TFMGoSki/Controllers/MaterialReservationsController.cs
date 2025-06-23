using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
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
            var materialReservations = await _context.MaterialReservations.ToListAsync();
            var userId = _userManager.GetUserId(User);
            var materialReservationsFiltered = materialReservations.Where(mr => mr.UserId.ToString() == userId);
            return View(materialReservationsFiltered);
        }

        // GET: MaterialReservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var materialReservation = await _context.MaterialReservations
                .FirstOrDefaultAsync(m => m.Id == id);

            if (materialReservation == null) return NotFound();

            var user = await _context.Users.FindAsync(materialReservation.UserId);

            var carts = await _context.ReservationMaterialCarts
                .Where(cart => cart.MaterialReservationId == materialReservation.Id)
                .Select(cart => new ReservationMaterialCartDto
                {
                    Id = cart.Id,
                    MaterialName = _context.Materials.FirstOrDefault(m => m.Id == cart.MaterialId)!.Name,
                    MaterialPrice = _context.Materials.FirstOrDefault(m => m.Id == cart.MaterialId)!.Price,
                    UserName = user.UserName,
                    NumberMaterialsBooked = cart.NumberMaterialsBooked,
                    ReservationTimeRangeMaterialDto = _context.ReservationTimeRangeMaterials
                        .Where(r => r.Id == cart.ReservationTimeRangeMaterialId)
                        .Select(r => new ReservationTimeRangeMaterialDto
                        {
                            Id = r.Id,
                            StartDateOnly = r.StartDateOnly,
                            EndDateOnly = r.EndDateOnly,
                            StartTimeOnly = r.StartTimeOnly,
                            EndTimeOnly = r.EndTimeOnly,
                            RemainingMaterialsQuantity = r.RemainingMaterialsQuantity,
                            MaterialId = _context.Materials.FirstOrDefault(m => m.Id == r.MaterialId)!.Name
                        }).FirstOrDefault()
                }).ToListAsync();

            var dto = new MaterialReservationDto
            {
                Id = materialReservation.Id,
                ClientName = user?.UserName ?? "Unknown",
                Total = (int)materialReservation.Total,
                reservationMaterialCartDto = carts
            };

            return View(dto);
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
            var materialReservationViewModel = new MaterialReservationViewModel()
            {
                Id = materialReservation.Id,
                UserId = materialReservation.UserId,
                Total = materialReservation.Total,
                Paid = materialReservation.Paid
            };
            return View(materialReservationViewModel);
        }

        // POST: MaterialReservations/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, MaterialReservationViewModel materialReservationViewModel)
        {
            if (id != materialReservationViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    MaterialReservation? materialReservation = _context.MaterialReservations.FirstOrDefault(mr => mr.Id == materialReservationViewModel.Id);
                    materialReservation.UpdatePaid(true);
                    _context.Update(materialReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialReservationExists(materialReservationViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
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
