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
    public class MaterialCommentsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public MaterialCommentsController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: MaterialComments
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaterialComments.ToListAsync());
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaterialComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationMaterialCartId,Id,Text,Raiting")] MaterialComment materialComment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(materialComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(materialComment);
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
            return View(materialComment);
        }

        // POST: MaterialComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationMaterialCartId,Id,Text,Raiting")] MaterialComment materialComment)
        {
            if (id != materialComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materialComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialCommentExists(materialComment.Id))
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
            return View(materialComment);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materialComment = await _context.MaterialComments.FindAsync(id);
            if (materialComment != null)
            {
                _context.MaterialComments.Remove(materialComment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialCommentExists(int id)
        {
            return _context.MaterialComments.Any(e => e.Id == id);
        }
    }
}
