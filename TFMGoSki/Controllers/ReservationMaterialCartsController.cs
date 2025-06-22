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
    public class ReservationMaterialCartsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationMaterialCartsController(TFMGoSkiDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ReservationMaterialCarts
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReservationMaterialCarts.ToListAsync());
        }
        public async Task<IActionResult> IndexUser()
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
            LoadSelectLists();
            ReservationMaterialCartViewModel reservationMaterialCartViewModel = new ReservationMaterialCartViewModel()
            {
                UserId = int.Parse(_userManager.GetUserId(User)),
                UserName = _context.Users.FirstOrDefault(u => u.Id == int.Parse(_userManager.GetUserId(User))).Email
            };
            return View();
        }

        // POST: ReservationMaterialCarts/Create
        [HttpPost]
        public async Task<IActionResult> Create(ReservationMaterialCartViewModel reservationMaterialCartViewModel)
        {
            reservationMaterialCartViewModel.UserId = int.Parse(_userManager.GetUserId(User));
            reservationMaterialCartViewModel.UserName = _context.Users.FirstOrDefault(u => u.Id == int.Parse(_userManager.GetUserId(User))).Email;
            if (ModelState.IsValid)
            {
                #region Remaining Materials Quantity

                var reservationTimeRangeMaterial = _context.ReservationTimeRangeMaterials
                    .FirstOrDefault(r => r.Id == reservationMaterialCartViewModel.ReservationTimeRangeMaterialId);
                var material = _context.Materials
                    .FirstOrDefault(c => c.Id == reservationMaterialCartViewModel.MaterialId);

                if (reservationTimeRangeMaterial == null || material == null)
                {
                    ModelState.AddModelError(string.Empty, "Reservation time range or material not found.");
                    LoadSelectLists();
                    return View(reservationMaterialCartViewModel);
                }

                int numberToBook = reservationMaterialCartViewModel.NumberMaterialsBooked;

                if (reservationTimeRangeMaterial.RemainingMaterialsQuantity == -1)
                {
                    ModelState.AddModelError(string.Empty, "There are no materials left in stock.");
                    LoadSelectLists(); 
                    return View(reservationMaterialCartViewModel);
                }

                // Si aún no se ha inicializado el número de plazas disponibles
                if (reservationTimeRangeMaterial.RemainingMaterialsQuantity == 0)
                {
                    reservationTimeRangeMaterial.RemainingMaterialsQuantity = material.QuantityMaterial;
                }

                // Verificamos que haya suficientes plazas disponibles
                if (reservationTimeRangeMaterial.RemainingMaterialsQuantity < numberToBook)
                {
                    ModelState.AddModelError(string.Empty, $"Only {reservationTimeRangeMaterial.RemainingMaterialsQuantity} materials are available.");
                    LoadSelectLists(); 
                    return View(reservationMaterialCartViewModel);
                }

                // Restamos las plazas reservadas
                reservationTimeRangeMaterial.RemainingMaterialsQuantity -= numberToBook;

                // Si ya no quedan plazas, marcamos con -1
                if (reservationTimeRangeMaterial.RemainingMaterialsQuantity == 0)
                {
                    reservationTimeRangeMaterial.RemainingMaterialsQuantity = -1;
                }

                _context.Update(reservationTimeRangeMaterial);
                await _context.SaveChangesAsync();

                #endregion

                ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(reservationMaterialCartViewModel.MaterialId, reservationMaterialCartViewModel.MaterialReservationId, reservationMaterialCartViewModel.UserId, reservationMaterialCartViewModel.ReservationTimeRangeMaterialId, reservationMaterialCartViewModel.NumberMaterialsBooked);
                _context.Add(reservationMaterialCart);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexUser));                
            }
            
            LoadSelectLists();
            return View(reservationMaterialCartViewModel);
        }
        private void LoadSelectLists()
        {
            ViewBag.MaterialId = new SelectList(_context.Materials, "Id", "Name");
            ViewBag.MaterialReservationId = new SelectList(_context.MaterialReservations, "Id", "Name");
            ViewBag.ReservationTimeRangeMaterialId = new SelectList(_context.ReservationTimeRangeMaterials, "Id", "Name");
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

            #region desplegable reservation time range

            var timeRanges = _context.ReservationTimeRangeMaterials
                .Where(r => r.MaterialId == reservationMaterialCart.MaterialId)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"{r.StartDateOnly:dd/MM/yyyy} {r.StartTimeOnly:hh\\:mm} - {r.EndDateOnly:dd/MM/yyyy} {r.EndTimeOnly:hh\\:mm}"
                }).ToList();

            ViewBag.TimeRanges = timeRanges;

            #endregion

            LoadSelectLists();
            ReservationMaterialCartViewModel viewModel = new ReservationMaterialCartViewModel();
            
            viewModel.Id = reservationMaterialCart.Id;
            viewModel.MaterialId = reservationMaterialCart.MaterialId;
            viewModel.MaterialReservationId = reservationMaterialCart.MaterialReservationId;
            viewModel.UserId = int.Parse(_userManager.GetUserId(User));
            viewModel.UserName = _context.Users.FirstOrDefault(u => u.Id == int.Parse(_userManager.GetUserId(User))).Email;
            viewModel.ReservationTimeRangeMaterialId = reservationMaterialCart.ReservationTimeRangeMaterialId;
            viewModel.NumberMaterialsBooked = reservationMaterialCart.NumberMaterialsBooked;
            return View(viewModel);
        }

        // POST: ReservationMaterialCarts/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReservationMaterialCartViewModel reservationMaterialCartViewModel)
        {
            reservationMaterialCartViewModel.UserId = int.Parse(_userManager.GetUserId(User));
            reservationMaterialCartViewModel.UserName = _context.Users.FirstOrDefault(u => u.Id == int.Parse(_userManager.GetUserId(User))).Email;
            if (id != reservationMaterialCartViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ReservationMaterialCart reservationMaterialCart = _context.ReservationMaterialCarts.FirstOrDefault(c => c.Id == id);

                    #region Remaining Materials Quantity

                    var originalTimeRangeId = reservationMaterialCart.ReservationTimeRangeMaterialId;
                    var originalMaterialId = reservationMaterialCart.MaterialId;
                    var originalNumberBooked = reservationMaterialCart.NumberMaterialsBooked;

                    var newTimeRangeId = reservationMaterialCartViewModel.ReservationTimeRangeMaterialId;
                    var newMaterialId = reservationMaterialCartViewModel.MaterialId;
                    var newNumberBooked = reservationMaterialCartViewModel.NumberMaterialsBooked;

                    var originalReservationTimeRange = await _context.ReservationTimeRangeMaterials
                        .FirstOrDefaultAsync(r => r.Id == originalTimeRangeId);
                    var newReservationTimeRange = await _context.ReservationTimeRangeMaterials
                        .FirstOrDefaultAsync(r => r.Id == newTimeRangeId);

                    var material = await _context.Materials.FirstOrDefaultAsync(c => c.Id == newMaterialId);

                    if (material == null || newReservationTimeRange == null || originalReservationTimeRange == null)
                    {
                        LoadSelectLists();

                        return View(reservationMaterialCartViewModel);
                    }

                    // Si se cambió el rango de horario
                    if (originalTimeRangeId != newTimeRangeId)
                    {
                        // Verificar disponibilidad en el nuevo rango
                        if (newReservationTimeRange.RemainingMaterialsQuantity == -1)
                        {
                            LoadSelectLists();

                            return View(reservationMaterialCartViewModel);
                        }

                        if (newReservationTimeRange.RemainingMaterialsQuantity == 0)
                        {
                            newReservationTimeRange.RemainingMaterialsQuantity = material.QuantityMaterial;
                        }

                        if (newReservationTimeRange.RemainingMaterialsQuantity < newNumberBooked)
                        {
                            ModelState.AddModelError(string.Empty, $"Only {newReservationTimeRange.RemainingMaterialsQuantity} materials available.");

                            LoadSelectLists();

                            return View(reservationMaterialCartViewModel);
                        }

                        // Restamos nuevas reservas
                        newReservationTimeRange.RemainingMaterialsQuantity -= newNumberBooked;
                        if (newReservationTimeRange.RemainingMaterialsQuantity == 0)
                        {
                            newReservationTimeRange.RemainingMaterialsQuantity = -1;
                        }

                        // Revertimos reservas originales
                        if (originalReservationTimeRange.RemainingMaterialsQuantity == -1)
                        {
                            originalReservationTimeRange.RemainingMaterialsQuantity = originalNumberBooked;
                        }
                        else
                        {
                            originalReservationTimeRange.RemainingMaterialsQuantity += originalNumberBooked;
                        }

                        _context.Update(originalReservationTimeRange);
                        _context.Update(newReservationTimeRange);
                        await _context.SaveChangesAsync();
                    }
                    // Si solo cambió la cantidad de personas pero el horario es el mismo
                    else if (originalNumberBooked != newNumberBooked)
                    {
                        int difference = newNumberBooked - originalNumberBooked;

                        if (difference > 0) // Quiere reservar más personas
                        {
                            if (newReservationTimeRange.RemainingMaterialsQuantity == -1)
                            {
                                ModelState.AddModelError(string.Empty, "There are no materials left in stock.");

                                LoadSelectLists();

                                return View(reservationMaterialCartViewModel);
                            }

                            if (newReservationTimeRange.RemainingMaterialsQuantity == 0)
                            {
                                newReservationTimeRange.RemainingMaterialsQuantity = material.QuantityMaterial;
                            }

                            if (newReservationTimeRange.RemainingMaterialsQuantity < difference)
                            {
                                ModelState.AddModelError(string.Empty, $"Only {newReservationTimeRange.RemainingMaterialsQuantity} more materials in stock.");

                                return View(reservationMaterialCartViewModel);
                            }

                            newReservationTimeRange.RemainingMaterialsQuantity -= difference;
                            if (newReservationTimeRange.RemainingMaterialsQuantity == 0)
                            {
                                newReservationTimeRange.RemainingMaterialsQuantity = -1;
                            }
                        }
                        else // Está reduciendo el número de personas
                        {
                            if (newReservationTimeRange.RemainingMaterialsQuantity == -1)
                            {
                                newReservationTimeRange.RemainingMaterialsQuantity = -difference;//Ahora es positivo
                            }
                            else
                            {
                                newReservationTimeRange.RemainingMaterialsQuantity += -difference;
                            }
                        }

                        _context.Update(newReservationTimeRange);
                        await _context.SaveChangesAsync();
                    }

                    #endregion

                
                    reservationMaterialCart.Update(reservationMaterialCartViewModel.MaterialId, reservationMaterialCartViewModel.MaterialReservationId, reservationMaterialCartViewModel.UserId, reservationMaterialCartViewModel.ReservationTimeRangeMaterialId, reservationMaterialCartViewModel.NumberMaterialsBooked);

                    _context.Update(reservationMaterialCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationMaterialCartExists(reservationMaterialCartViewModel.Id))
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
            LoadSelectLists();
            return View(reservationMaterialCartViewModel);
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

        [HttpGet]
        public IActionResult GetTimeRangesByMaterialId(int materialId)
        {
            var timeRanges = _context.ReservationTimeRangeMaterials
                .Where(r => r.MaterialId == materialId)
                .Select(r => new {
                    id = r.Id,
                    display = $"{r.StartDateOnly:dd/MM/yyyy} {r.StartTimeOnly:hh\\:mm} - {r.EndDateOnly:dd/MM/yyyy} {r.EndTimeOnly:hh\\:mm}"
                }).ToList();

            return Json(timeRanges);
        }

    }
}
