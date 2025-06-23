using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Exceptions;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class MaterialsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public MaterialsController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: Materials
        public async Task<IActionResult> Index()
        {
            return View(await _context.Materials.ToListAsync());
        }

        // GET: Materials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
            {
                return NotFound();
            }

            // Buscar las disponibilidades asociadas manualmente
            var reservations = await _context.ReservationTimeRangeMaterials
                .Where(r => r.MaterialId == material.Id)
                .ToListAsync();

            // Mapear a DTOs
            var reservationDtos = reservations.Select(r => new ReservationTimeRangeMaterialDto
            {
                Id = r.Id,
                StartDateOnly = r.StartDateOnly,
                EndDateOnly = r.EndDateOnly,
                StartTimeOnly = r.StartTimeOnly,
                EndTimeOnly = r.EndTimeOnly,
                RemainingMaterialsQuantity = r.RemainingMaterialsQuantity == -1 ? 0 : r.RemainingMaterialsQuantity,
                MaterialId = r.MaterialId.ToString()
            }).ToList();

            // Crear DTO de Material
            var materialDto = new MaterialDto
            {
                Id = material.Id,
                Name = material.Name,
                Description = material.Description,
                QuantityMaterial = material.QuantityMaterial,
                Price = material.Price,
                Size = material.Size,
                CityId = material.CityId.ToString(),
                MaterialTypeId = material.MaterialTypeId.ToString(),
                MaterialStatusId = material.MaterialStatusId.ToString(),
                ReservationTimeRangeMaterialDto = reservationDtos
            };

            return View(materialDto);
        }

        // GET: Materials/Create
        public IActionResult Create()
        {
            ViewBag.CityId = new SelectList(_context.Cities, "Id", "Name");
            ViewBag.MaterialTypeId = new SelectList(_context.MaterialTypes, "Id", "Name");
            ViewBag.MaterialStatusId = new SelectList(_context.MaterialStatuses, "Id", "Name");
            return View();
        }

        // POST: Materials/Create
        [HttpPost]
        public async Task<IActionResult> Create(MaterialViewModel materialViewModel)
        {
            var duplicate = _context.Materials
                .FirstOrDefault(c => c.Name.Equals(materialViewModel.Name));

            if (duplicate != null)
            {
                ModelState.AddModelError("Name", "There is already a material type with this name.");
                LoadSelectLists();
                return View(materialViewModel);
            }

            if (ModelState.IsValid)
            {
                Material material = new Material(materialViewModel.Name, materialViewModel.Description, materialViewModel.QuantityMaterial.Value, materialViewModel.Price.Value, materialViewModel.Size, materialViewModel.CityId.Value, materialViewModel.MaterialTypeId.Value, materialViewModel.MaterialStatusId.Value);
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            LoadSelectLists();
            return View(materialViewModel);
        }
        private void LoadSelectLists()
        {
            ViewBag.CityId = new SelectList(_context.Cities, "Id", "Name");
            ViewBag.MaterialTypeId = new SelectList(_context.MaterialTypes, "Id", "Name");
            ViewBag.MaterialStatusId = new SelectList(_context.MaterialStatuses, "Id", "Name");
        }

        // GET: Materials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            MaterialViewModel materialViewModel = new MaterialViewModel()
            {
                Id = material.Id,
                Name = material.Name,
                Description = material.Description,
                QuantityMaterial = material.QuantityMaterial,
                Price = material.Price,
                Size = material.Size,
                CityId = material.CityId,
                MaterialTypeId = material.MaterialTypeId,
                MaterialStatusId = material.MaterialStatusId
            };

            LoadSelectLists();

            return View(materialViewModel);
        }

        // POST: Materials/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, MaterialViewModel materialViewModel)
        {
            if (id != materialViewModel.Id)
            {
                return NotFound();
            }

            var duplicate = _context.Materials
                .FirstOrDefault(c => c.Name.Equals(materialViewModel.Name) && c.Id != id);

            if (duplicate != null)
            {
                ModelState.AddModelError("Name", "There is already a material type with this name.");
                LoadSelectLists();
                return View(materialViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Material material = _context.Materials.FirstOrDefault(m => m.Id == materialViewModel.Id);
                    material.Update(materialViewModel.Name, materialViewModel.Description, materialViewModel.QuantityMaterial.Value, materialViewModel.Price.Value, materialViewModel.Size, materialViewModel.CityId.Value, materialViewModel.MaterialTypeId.Value, materialViewModel.MaterialStatusId.Value);
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(materialViewModel.Id))
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
            LoadSelectLists();
            return View(materialViewModel);
        }

        // GET: Materials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.Id == id);
        }
    }
}
