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
    public class MaterialCommentsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;
        private readonly UserManager<User> _userManager;

        public MaterialCommentsController(TFMGoSkiDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MaterialComments
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaterialComments.ToListAsync());
        }

        // GET: MaterialComments
        public async Task<IActionResult> IndexUser()
        {
            var userId = int.Parse(_userManager.GetUserId(User));

            var comments = await _context.MaterialComments
                .Where(c => _context.ReservationMaterialCarts
                    .Any(rmc => rmc.Id == c.ReservationMaterialCartId && rmc.UserId == userId))
                .ToListAsync();

            var cartIds = comments.Select(c => c.ReservationMaterialCartId).Distinct().ToList();

            var carts = await _context.ReservationMaterialCarts
                .Where(c => cartIds.Contains(c.Id))
                .ToListAsync();

            var materialIds = carts.Select(c => c.MaterialId).Distinct().ToList();

            var materials = await _context.Materials
                .Where(m => materialIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            var dtos = comments.Select(c =>
            {
                var cart = carts.FirstOrDefault(rmc => rmc.Id == c.ReservationMaterialCartId);
                var materialName = cart != null && materials.ContainsKey(cart.MaterialId)
                    ? materials[cart.MaterialId].Name
                    : "Unknown";

                return new MaterialCommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    Raiting = c.Raiting,
                    ReservationMaterialCartName = materialName
                };
            }).ToList();

            return View(dtos);
        }


        // GET: MaterialComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialComment = await _context.MaterialComments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialComment == null)
            {
                return NotFound();
            }

            return View(materialComment);
        }

        // GET: MaterialComments/Create
        public async Task<IActionResult> Create(int? reservationMaterialCartId = null) //Viene de ReservationMaterialCart
        {
            var reservation = await _context.ReservationMaterialCarts.FirstOrDefaultAsync(r => r.Id == reservationMaterialCartId);
            if (reservation == null)
                return NotFound();

            var viewModel = new MaterialCommentViewModel
            {
                ReservationMaterialCartId = reservation.Id,
                ReservationMaterialCartName = GenerarNombreDeMaterial(reservation)
            };

            return View(viewModel);
        }

        // POST: MaterialComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] 
        public async Task<IActionResult> Create(int reservationMaterialCartId, MaterialCommentViewModel materialCommentViewModel)
        {
            var existingComment = await _context.MaterialComments
                .FirstOrDefaultAsync(c => c.ReservationMaterialCartId == reservationMaterialCartId);

            if (existingComment != null) //Error comentario ya existe
            {
                ModelState.AddModelError(string.Empty, "A comment already exists for this reservation.");

                ReservationMaterialCart? reservationCommented = await _context.ReservationMaterialCarts.FirstOrDefaultAsync(r => r.Id == reservationMaterialCartId);

                materialCommentViewModel.ReservationMaterialCartName = GenerarNombreDeMaterial(reservationCommented);

                return View(materialCommentViewModel);
            }

            MaterialComment materialComment = new MaterialComment(materialCommentViewModel.ReservationMaterialCartId, materialCommentViewModel.Text, materialCommentViewModel.Raiting);
            if (ModelState.IsValid)
            {
                _context.Add(materialComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexUser));
            }
           
            return View(materialComment);
        }

        private string GenerarNombreDeMaterial(ReservationMaterialCart reservation)
        {
            var material = _context.Materials.First(c => c.Id == reservation.MaterialId);
            var range = _context.ReservationTimeRangeMaterials.First(r => r.Id == reservation.ReservationTimeRangeMaterialId);
            return $"{material.Name} - {range.StartDateOnly} - {range.EndDateOnly}";
        }

        // GET: MaterialComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialComment = await _context.MaterialComments.FindAsync(id);
            if (materialComment == null)
            {
                return NotFound();
            }

            var reservationMaterialCart = await _context.ReservationMaterialCarts.FindAsync(materialComment.ReservationMaterialCartId);

            var viewModel = new MaterialCommentViewModel
            {
                Id = materialComment.Id,
                ReservationMaterialCartId = materialComment.ReservationMaterialCartId,
                ReservationMaterialCartName = GenerarNombreDeMaterial(reservationMaterialCart),
                Text = materialComment.Text,
                Raiting = materialComment.Raiting
            };

            return View(viewModel);
        }


        // POST: MaterialComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, MaterialCommentViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var materialComment = await _context.MaterialComments.FindAsync(id);
                    if (materialComment == null)
                    {
                        return NotFound();
                    }

                    materialComment.ReservationMaterialCartId = viewModel.ReservationMaterialCartId;
                    materialComment.Text = viewModel.Text;
                    materialComment.Raiting = viewModel.Raiting;

                    _context.Update(materialComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialCommentExists(viewModel.Id))
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

            var reservationMaterialCart = await _context.ReservationMaterialCarts.FindAsync(viewModel.ReservationMaterialCartId);

            viewModel.ReservationMaterialCartName = GenerarNombreDeMaterial(reservationMaterialCart);

            return View(viewModel);
        }



        // GET: MaterialComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialComment = await _context.MaterialComments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialComment == null)
            {
                return NotFound();
            }

            return View(materialComment);
        }

        // POST: MaterialComments/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materialComment = await _context.MaterialComments.FindAsync(id);
            if (materialComment != null)
            {
                _context.MaterialComments.Remove(materialComment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexUser));
        }

        private bool MaterialCommentExists(int id)
        {
            return _context.MaterialComments.Any(e => e.Id == id);
        }
    }
}
