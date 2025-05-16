using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Models;

namespace TFMGoSki.Controllers
{
    public class ClassCommentsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public ClassCommentsController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: ClassComments
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClassComments.ToListAsync());
        }

        // GET: ClassComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classComment = await _context.ClassComments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classComment == null)
            {
                return NotFound();
            }

            return View(classComment);
        }

        // GET: ClassComments/Create
        public async Task<IActionResult> Create()
        {
            var dbSetClassReservation = await _context.ClassReservations.ToListAsync();
            var listaClienteClase = new List<KeyValuePair<int, string>>();

            foreach (var classReservation in dbSetClassReservation)
            {
                var client = await _context.Users.FirstOrDefaultAsync(c => c.Id == classReservation.UserId);
                var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);

                if (client != null && @class != null)
                {
                    string displayText = $"{client.Name} - {@class.Name}";
                    listaClienteClase.Add(new KeyValuePair<int, string>(classReservation.Id, displayText));
                }
            }

            ViewBag.ClassReservationId = new SelectList(listaClienteClase, "Key", "Value");

            return View();
        }

        // POST: ClassComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(int classReservationId, string text , int raiting)
        {
            ClassComment classComment = new ClassComment(classReservationId, text, raiting);
            if (ModelState.IsValid)
            {
                _context.Add(classComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classComment);
        }

        // GET: ClassComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classComment = await _context.ClassComments.FindAsync(id);
            if (classComment == null)
            {
                return NotFound();
            }

            var classReservations = await _context.ClassReservations.ToListAsync();
            var listaClienteClase = new List<KeyValuePair<int, string>>();

            foreach (var classReservation in classReservations)
            {
                var client = await _context.Users.FirstOrDefaultAsync(c => c.Id == classReservation.UserId);
                var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);

                if (client != null && @class != null)
                {
                    string displayText = $"{client.Name} - {@class.Name}";
                    listaClienteClase.Add(new KeyValuePair<int, string>(classReservation.Id, displayText));
                }
            }

            ViewBag.ClassReservationId = new SelectList(listaClienteClase, "Key", "Value", classComment.ClassReservationId);

            return View(classComment);
        }

        // POST: ClassComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int classReservationId, string text, int raiting)
        {
            ClassComment? classComment = _context.ClassComments.FirstOrDefault(c => c.Id == id); 

            if (ModelState.IsValid)
            {
                try
                {
                    classComment.Update(classReservationId, text, raiting);
                    _context.Update(classComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassCommentExists(classComment.Id))
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
            return View(classComment);
        }

        // GET: ClassComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classComment = await _context.ClassComments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classComment == null)
            {
                return NotFound();
            }

            return View(classComment);
        }

        // POST: ClassComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classComment = await _context.ClassComments.FindAsync(id);
            if (classComment != null)
            {
                _context.ClassComments.Remove(classComment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassCommentExists(int id)
        {
            return _context.ClassComments.Any(e => e.Id == id);
        }
    }
}
