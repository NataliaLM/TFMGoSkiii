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
            var items = await _context.ReservationTimeRangeMaterials.ToListAsync();

            var dtoList = items.Select(item => new ReservationTimeRangeMaterialDto
            {
                Id = item.Id,
                StartDateOnly = item.StartDateOnly,
                EndDateOnly = item.EndDateOnly,
                StartTimeOnly = item.StartTimeOnly,
                EndTimeOnly = item.EndTimeOnly,
                RemainingMaterialsQuantity = item.RemainingMaterialsQuantity < 0 ? 0 : item.RemainingMaterialsQuantity,
                MaterialId = _context.Materials
                    .FirstOrDefault(m => m.Id == item.MaterialId)?.Name ?? "Unknown"
            }).ToList();

            return View(dtoList);
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
        public IActionResult Create(int materialId)
        {
            var material = _context.Materials.FirstOrDefault(m => m.Id == materialId);
            if (material == null)
            {
                return NotFound();
            }

            var viewModel = new ReservationTimeRangeMaterialViewModel
            {
                MaterialId = material.Id,
                MaterialName = material.Name // debes agregar esta propiedad al ViewModel
            };

            return View(viewModel);
        }


        // POST: ReservationTimeRangeMaterials/Create
        [HttpPost]
        public async Task<IActionResult> Create(int materialId, ReservationTimeRangeMaterialViewModel reservationTimeRangeMaterialViewModel)
        {
            ViewBag.MaterialId = new SelectList(_context.Materials, "Id", "Name");
            if (ModelState.IsValid)
            {
                Material? material = _context.Materials.FirstOrDefault(m => m.Id == materialId);
                ReservationTimeRangeMaterial reservationTimeRangeMaterial = new ReservationTimeRangeMaterial(reservationTimeRangeMaterialViewModel.StartDateOnly, reservationTimeRangeMaterialViewModel.EndDateOnly, reservationTimeRangeMaterialViewModel.StartTimeOnly, reservationTimeRangeMaterialViewModel.EndTimeOnly, material.QuantityMaterial, reservationTimeRangeMaterialViewModel.MaterialId);
                _context.Add(reservationTimeRangeMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservationTimeRangeMaterialViewModel);
        }

        // GET: ReservationTimeRangeMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.ReservationTimeRangeMaterials.FindAsync(id);
            if (entity == null) return NotFound();

            ViewBag.MaterialId = new SelectList(_context.Materials, "Id", "Name", entity.MaterialId);
            Material? material = _context.Materials.FirstOrDefault(m => m.Id == entity.MaterialId);
            var viewModel = new ReservationTimeRangeMaterialViewModel
            {
                Id = entity.Id,
                StartDateOnly = entity.StartDateOnly,
                EndDateOnly = entity.EndDateOnly,
                StartTimeOnly = entity.StartTimeOnly,
                EndTimeOnly = entity.EndTimeOnly,
                RemainingMaterialsQuantity = entity.RemainingMaterialsQuantity == -1 ? 0 : entity.RemainingMaterialsQuantity,
                MaterialId = entity.MaterialId
            };

            return View(viewModel);
        }


        // POST: ReservationTimeRangeMaterials/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReservationTimeRangeMaterialViewModel reservationTimeRangeMaterialViewModel)
        {
            if (id != reservationTimeRangeMaterialViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Material? material = _context.Materials.FirstOrDefault(m => m.Id == reservationTimeRangeMaterialViewModel.MaterialId);

                    ReservationTimeRangeMaterial? reservationTimeRangeMaterial = _context.ReservationTimeRangeMaterials.FirstOrDefault(r => r.Id == reservationTimeRangeMaterialViewModel.Id);
                    reservationTimeRangeMaterial.Update(reservationTimeRangeMaterialViewModel.StartDateOnly, reservationTimeRangeMaterialViewModel.EndDateOnly, reservationTimeRangeMaterialViewModel.StartTimeOnly, reservationTimeRangeMaterialViewModel.EndTimeOnly, material.QuantityMaterial, reservationTimeRangeMaterialViewModel.MaterialId);

                    _context.Update(reservationTimeRangeMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationTimeRangeMaterialExists(reservationTimeRangeMaterialViewModel.Id))
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
            ViewBag.MaterialId = new SelectList(_context.Materials, "Id", "Name", reservationTimeRangeMaterialViewModel.MaterialId);

            return View(reservationTimeRangeMaterialViewModel);
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
