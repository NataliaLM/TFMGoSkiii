using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
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
            var userId = int.Parse(_userManager.GetUserId(User));

            // Obtiene los carts activos no pagados
            var carts = await _context.ReservationMaterialCarts
                .Where(cart => cart.UserId == userId &&
                               _context.MaterialReservations
                                   .Any(res => res.Id == cart.MaterialReservationId && !res.Paid))
                .ToListAsync();

            var materialIds = carts.Select(c => c.MaterialId).Distinct().ToList();
            var reservationIds = carts.Select(c => c.MaterialReservationId).Distinct().ToList();
            var timeRangeIds = carts.Select(c => c.ReservationTimeRangeMaterialId).Distinct().ToList();

            var materials = await _context.Materials
                .Where(m => materialIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            var reservations = await _context.MaterialReservations
                .Where(r => reservationIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id);

            var timeRanges = await _context.ReservationTimeRangeMaterials
                .Where(rt => timeRangeIds.Contains(rt.Id))
                .ToDictionaryAsync(rt => rt.Id);

            var user = await _userManager.GetUserAsync(User);

            var dtos = new List<ReservationMaterialCartDto>();

            foreach (var cart in carts)
            {
                var dto = new ReservationMaterialCartDto
                {
                    Id = cart.Id,
                    MaterialName = materials.ContainsKey(cart.MaterialId) ? materials[cart.MaterialId].Name : null,
                    MaterialPrice = materials.ContainsKey(cart.MaterialId) ? materials[cart.MaterialId].Price : null,
                    MaterialReservationName = reservations.ContainsKey(cart.MaterialReservationId)
                        ? $"Paid {reservations[cart.MaterialReservationId].Paid} - Total {reservations[cart.MaterialReservationId].Total}"
                        : null,
                    UserName = user.UserName,
                    ReservationTimeRangeMaterialDto = timeRanges.ContainsKey(cart.ReservationTimeRangeMaterialId)
                        ? new ReservationTimeRangeMaterialDto
                        {
                            StartDateOnly = timeRanges[cart.ReservationTimeRangeMaterialId].StartDateOnly,
                            EndDateOnly = timeRanges[cart.ReservationTimeRangeMaterialId].EndDateOnly,
                            StartTimeOnly = timeRanges[cart.ReservationTimeRangeMaterialId].StartTimeOnly,
                            EndTimeOnly = timeRanges[cart.ReservationTimeRangeMaterialId].EndTimeOnly,
                            RemainingMaterialsQuantity = timeRanges[cart.ReservationTimeRangeMaterialId].RemainingMaterialsQuantity
                        }
                        : null,
                    NumberMaterialsBooked = cart.NumberMaterialsBooked,
                    HasComment = await _context.MaterialComments
                        .AnyAsync(c => c.ReservationMaterialCartId == cart.Id)
                };

                dtos.Add(dto);
            }

            return View(dtos);

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
        public IActionResult Create(int? materialId, int? reservationTimeRangeMaterialId)
        {
            var userId = int.Parse(_userManager.GetUserId(User));
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            // Buscar o crear una reserva base del usuario
            var userReservation = _context.MaterialReservations
                .FirstOrDefault(r => r.UserId == userId && !r.Paid);

            if (userReservation == null)
            {
                userReservation = new MaterialReservation(userId, 0, false);
                _context.MaterialReservations.Add(userReservation);
                _context.SaveChanges();
            }

            //LoadSelectLists(userId); // Solo cargar reservas del usuario

            Material? material = _context.Materials.FirstOrDefault(m => m.Id == materialId);
            if (material == null) return NotFound();
            ReservationTimeRangeMaterial? reservationTimeRangeMaterial = _context.ReservationTimeRangeMaterials.FirstOrDefault(r => r.Id == reservationTimeRangeMaterialId);
            if (reservationTimeRangeMaterial == null) return NotFound();            

            var viewModel = new ReservationMaterialCartViewModel
            {
                UserId = userId,
                UserName = user?.Email,
                MaterialId = materialId ?? 0,
                MaterialName = $"{material.Name} - {material.Size} - {material.Price} - {material.Description}",
                ReservationTimeRangeMaterialId = reservationTimeRangeMaterialId ?? 0,
                ReservationTimeRangeMaterialName = $"({reservationTimeRangeMaterial.EndDateOnly} - {reservationTimeRangeMaterial.EndDateOnly}) - ({reservationTimeRangeMaterial.StartTimeOnly} - {reservationTimeRangeMaterial.EndTimeOnly})",
                MaterialReservationId = userReservation.Id,
                MaterialReservationName = $"{userReservation.Paid} - {userReservation.Total}"
            };

            return View(viewModel);
        }


        // POST: ReservationMaterialCarts/Create
        [HttpPost]
        public async Task<IActionResult> Create(ReservationMaterialCartViewModel reservationMaterialCartViewModel)
        {
            int userId = int.Parse(_userManager.GetUserId(User));
            reservationMaterialCartViewModel.UserId = userId;
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            string? reservationMaterial = user.Email;
            if (reservationMaterial == null) return NotFound();
            reservationMaterialCartViewModel.UserName = reservationMaterial;
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
                    //LoadSelectLists();
                    return View(reservationMaterialCartViewModel);
                }

                int numberToBook = reservationMaterialCartViewModel.NumberMaterialsBooked;

                if (reservationTimeRangeMaterial.RemainingMaterialsQuantity == -1)
                {
                    ModelState.AddModelError(string.Empty, "There are no materials left in stock.");
                    //LoadSelectLists(); 
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
                    //LoadSelectLists(); 
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

                #region Total

                var userReservation = _context.MaterialReservations.FirstOrDefault(mr => mr.Id == reservationMaterialCartViewModel.MaterialReservationId);
                if (userReservation == null) return NotFound();
                userReservation.Total = userReservation.Total + (material.Price * reservationMaterialCartViewModel.NumberMaterialsBooked);
                _context.Update(userReservation);
                await _context.SaveChangesAsync();

                #endregion
                 
                ReservationMaterialCart reservationMaterialCart = new ReservationMaterialCart(reservationMaterialCartViewModel.MaterialId, reservationMaterialCartViewModel.MaterialReservationId, reservationMaterialCartViewModel.UserId, reservationMaterialCartViewModel.ReservationTimeRangeMaterialId, reservationMaterialCartViewModel.NumberMaterialsBooked);
                _context.Add(reservationMaterialCart);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexUser));                
            }
            
            //LoadSelectLists();
            return View(reservationMaterialCartViewModel);
        }
        private void LoadSelectLists()
        {
            ViewBag.MaterialId = new SelectList(_context.Materials, "Id", "Name");
            ViewBag.MaterialReservationId = new SelectList(_context.MaterialReservations, "Id", "Paid");
            ViewBag.ReservationTimeRangeMaterialId = new SelectList(_context.ReservationTimeRangeMaterials, "Id", "StartDateOnly");
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

            var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == reservationMaterialCart.MaterialId);
            var timeRange = await _context.ReservationTimeRangeMaterials.FirstOrDefaultAsync(r => r.Id == reservationMaterialCart.ReservationTimeRangeMaterialId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == reservationMaterialCart.UserId);

            var viewModel = new ReservationMaterialCartViewModel
            {
                Id = reservationMaterialCart.Id,
                MaterialId = reservationMaterialCart.MaterialId,
                MaterialName = $"{material.Name} - {material.Size} - {material.Price} - {material.Description}",
                MaterialReservationId = reservationMaterialCart.MaterialReservationId,
                ReservationTimeRangeMaterialId = reservationMaterialCart.ReservationTimeRangeMaterialId,
                ReservationTimeRangeMaterialName = $"{timeRange.StartDateOnly:dd/MM/yyyy} - {timeRange.EndDateOnly:dd/MM/yyyy} ({timeRange.StartTimeOnly:hh\\:mm} - {timeRange.EndTimeOnly:hh\\:mm})",
                UserId = reservationMaterialCart.UserId,
                UserName = user?.Email,
                NumberMaterialsBooked = reservationMaterialCart.NumberMaterialsBooked
            };

            return View(viewModel);
        }

        // POST: ReservationMaterialCarts/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReservationMaterialCartViewModel reservationMaterialCartViewModel)
        {
            int userId = int.Parse(_userManager.GetUserId(User));
            reservationMaterialCartViewModel.UserId = userId;
            var userFound = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (userFound != null) return NotFound();
            string? user = userFound.Email;
            if (user == null) return NotFound();
            reservationMaterialCartViewModel.UserName = user;
            if (id != reservationMaterialCartViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ReservationMaterialCart reservationMaterialCart = _context.ReservationMaterialCarts.FirstOrDefault(c => c.Id == id);
                    if (reservationMaterialCart == null) return NotFound();

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

                    #region Total

                    var userReservation = _context.MaterialReservations.FirstOrDefault(mr => mr.Id == reservationMaterialCart.MaterialReservationId);
                    if (userReservation == null) return NotFound();

                    int differenceChangeReservation = newNumberBooked - originalNumberBooked;

                    if (differenceChangeReservation != 0)
                    {
                        decimal totalChange = material.Price * differenceChangeReservation;
                        userReservation.Total += totalChange;
                        _context.Update(userReservation);
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
            if (reservationMaterialCart == null) return NotFound();

            return View(reservationMaterialCart);
        }

        // POST: ReservationMaterialCarts/Delete/5
        [HttpPost, ActionName("Delete")] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservationMaterialCart = await _context.ReservationMaterialCarts.FindAsync(id);
            if (reservationMaterialCart == null) return NotFound();
            
            #region Total

            var material = _context.Materials.FirstOrDefault(c => c.Id == reservationMaterialCart.MaterialId);
            if (material == null) return NotFound();

            var userReservation = _context.MaterialReservations.FirstOrDefault(mr => mr.Id == reservationMaterialCart.MaterialReservationId);
            if (userReservation == null) return NotFound();
            userReservation.Total = userReservation.Total - (material.Price * reservationMaterialCart.NumberMaterialsBooked);
            _context.Update(userReservation);
            await _context.SaveChangesAsync();

            #endregion

            #region RemainigStudentsQuantity

            ReservationTimeRangeMaterial? reservationTimeRangeMaterial = _context.ReservationTimeRangeMaterials.FirstOrDefault(rtrm => rtrm.Id == reservationMaterialCart.ReservationTimeRangeMaterialId);
            if (reservationTimeRangeMaterial == null) return NotFound();
            if (reservationTimeRangeMaterial.RemainingMaterialsQuantity == -1)
            {
                reservationTimeRangeMaterial.RemainingMaterialsQuantity = reservationMaterialCart.NumberMaterialsBooked;
            }
            else
            {
                reservationTimeRangeMaterial.RemainingMaterialsQuantity += reservationMaterialCart.NumberMaterialsBooked;
            }

            _context.Update(reservationTimeRangeMaterial);
            _context.SaveChanges();

            #endregion
            
            if (reservationMaterialCart != null)
            {
                _context.ReservationMaterialCarts.Remove(reservationMaterialCart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexUser));
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
