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
            if (User.IsInRole("Client"))
            {
                ViewData["IsClient"] = true;
            }
            else
            {
                ViewData["IsClient"] = false;
            }

            if (id == null) return NotFound();

            var materialReservation = await _context.MaterialReservations
                .FirstOrDefaultAsync(m => m.Id == id);

            if (materialReservation == null) return NotFound();

            var user = await _context.Users.FindAsync(materialReservation.UserId);

            var cartsRaw = await _context.ReservationMaterialCarts
                .Where(c => c.MaterialReservationId == materialReservation.Id)
                .ToListAsync();

            var materialIds = cartsRaw.Select(c => c.MaterialId).Distinct();
            var timeRangeIds = cartsRaw.Select(c => c.ReservationTimeRangeMaterialId).Distinct();

            var materials = await _context.Materials
                .Where(m => materialIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            var timeRanges = await _context.ReservationTimeRangeMaterials
                .Where(r => timeRangeIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id);

            // Optionally, load material comments to check if one already exists
            var commentIds = await _context.MaterialComments
                .Select(c => c.ReservationMaterialCartId)
                .ToListAsync();

            var cartDtos = cartsRaw.Select(cart => new ReservationMaterialCartDto
            {
                Id = cart.Id,
                MaterialName = materials[cart.MaterialId].Name,
                MaterialPrice = materials[cart.MaterialId].Price,
                UserName = user?.UserName ?? "Unknown",
                NumberMaterialsBooked = cart.NumberMaterialsBooked,
                HasComment = commentIds.Contains(cart.Id), // optional flag
                ReservationTimeRangeMaterialDto = new ReservationTimeRangeMaterialDto
                {
                    Id = timeRanges[cart.ReservationTimeRangeMaterialId].Id,
                    StartDateOnly = timeRanges[cart.ReservationTimeRangeMaterialId].StartDateOnly,
                    EndDateOnly = timeRanges[cart.ReservationTimeRangeMaterialId].EndDateOnly,
                    StartTimeOnly = timeRanges[cart.ReservationTimeRangeMaterialId].StartTimeOnly,
                    EndTimeOnly = timeRanges[cart.ReservationTimeRangeMaterialId].EndTimeOnly,
                    RemainingMaterialsQuantity = timeRanges[cart.ReservationTimeRangeMaterialId].RemainingMaterialsQuantity,
                    MaterialId = materials[cart.MaterialId].Name // nombre en lugar de ID
                }
            }).ToList();

            var dto = new MaterialReservationDto
            {
                Id = materialReservation.Id,
                ClientName = user?.UserName ?? "Unknown",
                Total = (int)materialReservation.Total,
                reservationMaterialCartDto = cartDtos
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
            return RedirectToAction(nameof(IndexUser));
        }

        private bool MaterialReservationExists(int id)
        {
            return _context.MaterialReservations.Any(e => e.Id == id);
        }
    }
}
