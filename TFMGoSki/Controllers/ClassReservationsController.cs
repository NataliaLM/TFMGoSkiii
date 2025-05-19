using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Models;

namespace TFMGoSki.Controllers
{
    public class ClassReservationsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public ClassReservationsController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: ClassReservations
        public async Task<IActionResult> Index()
        {
            var classReservations = await _context.ClassReservations.ToListAsync();

            var listClassReservationDto = new List<ClassReservationDto>();

            foreach (var reservation in classReservations)
            {
                var client = await _context.Users.FirstOrDefaultAsync(c => c.Id == reservation.UserId);
                var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == reservation.ClassId);

                if (client != null && @class != null)
                {
                    listClassReservationDto.Add(new ClassReservationDto
                    {
                        Id = reservation.Id,
                        ClientName = client.UserName,
                        ClassName = @class.Name
                    });
                }
            }

            return View(listClassReservationDto);
        }

        // GET: ClassReservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classReservation = await _context.ClassReservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classReservation == null)
            {
                return NotFound();
            }

            return View(classReservation);
        }

        // GET: ClassReservations/Create
        public IActionResult Create()
        {
            ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name");
            return View();
        }

        // POST: ClassReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int userId, int classId)
        {
            ClassReservation classReservation = new ClassReservation(userId, classId);
            if (ModelState.IsValid)
            {
                _context.Add(classReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classReservation);
        }

        // GET: ClassReservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classReservation = await _context.ClassReservations.FindAsync(id);
            if (classReservation == null)
            {
                return NotFound();
            }

            ViewBag.ClientId = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name");

            return View(classReservation);
        }

        // POST: ClassReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, int userId, int classId)
        {
            ClassReservation? classReservation = _context.ClassReservations.FirstOrDefault(c => c.Id == id);
            if (ModelState.IsValid)
            {
                try
                {
                    classReservation.Update(userId, classId);
                    _context.Update(classReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassReservationExists(classReservation.Id))
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
            return View(classReservation);
        }

        // GET: ClassReservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classReservation = await _context.ClassReservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classReservation == null)
            {
                return NotFound();
            }

            return View(classReservation);
        }

        // POST: ClassReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classReservation = await _context.ClassReservations.FindAsync(id);
            if (classReservation != null)
            {
                _context.ClassReservations.Remove(classReservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassReservationExists(int id)
        {
            return _context.ClassReservations.Any(e => e.Id == id);
        }
    }
}
