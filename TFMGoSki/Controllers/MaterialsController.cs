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

        // GET: Materials Index User
        public async Task<IActionResult> IndexUser(decimal? minPrice, decimal? maxPrice, string? size, double? minRating)
        {
            ViewBag.Sizes = await _context.Materials
    .Select(m => m.Size)
    .Distinct()
    .Where(s => s != null)
    .ToListAsync();

            var materials = await _context.Materials.ToListAsync();

            var materialIds = materials.Select(m => m.Id).ToList();

            var materialComments = await _context.MaterialComments
                .Join(_context.ReservationMaterialCarts,
                      comment => comment.ReservationMaterialCartId,
                      cart => cart.Id,
                      (comment, cart) => new { comment, cart.MaterialId })
                .Where(x => materialIds.Contains(x.MaterialId))
                .ToListAsync();

            var groupedComments = materialComments
                .GroupBy(x => x.MaterialId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new MaterialCommentDto
                    {
                        Text = x.comment.Text,
                        Raiting = x.comment.Raiting
                    }).ToList()
                );

            var dtoList = materials.Select(material => new MaterialDto
            {
                Id = material.Id,
                Name = material.Name,
                Description = material.Description,
                QuantityMaterial = material.QuantityMaterial,
                Price = material.Price,
                Size = material.Size,
                Comments = groupedComments.ContainsKey(material.Id)
                    ? groupedComments[material.Id]
                    : new List<MaterialCommentDto>()
            }).ToList();

            // Aplica filtros
            if (minPrice.HasValue)
                dtoList = dtoList.Where(m => m.Price >= minPrice.Value).ToList();

            if (maxPrice.HasValue)
                dtoList = dtoList.Where(m => m.Price <= maxPrice.Value).ToList();

            if (!string.IsNullOrWhiteSpace(size))
                dtoList = dtoList.Where(m => m.Size != null && m.Size.Equals(size, StringComparison.OrdinalIgnoreCase)).ToList();

            if (minRating.HasValue)
                dtoList = dtoList.Where(m => m.Comments.Any() && m.Comments.Average(c => c.Raiting) >= minRating.Value).ToList();

            return View(dtoList);
        }

        // GET: Materials/Details/5
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

            #region comments

            var materials = await _context.Materials.ToListAsync();

            var materialIds = materials.Select(m => m.Id).ToList();

            var materialCommentDtos = await _context.MaterialComments
    .Join(_context.ReservationMaterialCarts,
          comment => comment.ReservationMaterialCartId,
          cart => cart.Id,
          (comment, cart) => new { comment, cart })
    .Where(x => x.cart.MaterialId == material.Id)
    .Select(x => new MaterialCommentDto
    {
        Id = x.comment.Id,
        Text = x.comment.Text,
        Raiting = x.comment.Raiting
    })
    .ToListAsync();

            #endregion

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
                ReservationTimeRangeMaterialDto = reservationDtos,
                Comments = materialCommentDtos
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
                    if (material == null) return NotFound();
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
