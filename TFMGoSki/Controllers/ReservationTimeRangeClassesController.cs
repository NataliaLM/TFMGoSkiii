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
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class ReservationTimeRangeClassesController : Controller
    {
        private readonly TFMGoSkiDbContext _context;

        public ReservationTimeRangeClassesController(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        // GET: ReservationTimeRangeClasses
        public async Task<IActionResult> Index()
        {
            List<ReservationTimeRangeClass>? reservationTimeRangeClasses = await _context.ReservationTimeRangeClasses.ToListAsync();
            List<ReservationTimeRangeClassDto> reservationTimeRangeClassDtos = new List<ReservationTimeRangeClassDto>();

            foreach (ReservationTimeRangeClass? reservationTimeRangeClass in reservationTimeRangeClasses)
            {
                Class? @class = _context.Classes.FirstOrDefault(c => c.Id == reservationTimeRangeClass.ClassId);

                ReservationTimeRangeClassDto reservationTimeRangeClassDto = new ReservationTimeRangeClassDto
                {
                    Id = reservationTimeRangeClass.Id,
                    RemainingStudentsQuantity = reservationTimeRangeClass.RemainingStudentsQuantity,
                    Class = @class.Name,
                    StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                    EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                    StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                    EndTimeOnly = reservationTimeRangeClass.EndTimeOnly,
                };

                reservationTimeRangeClassDtos.Add(reservationTimeRangeClassDto);
            }

            return View(reservationTimeRangeClassDtos);
        }

        // GET: ReservationTimeRangeClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationTimeRangeClass = await _context.ReservationTimeRangeClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationTimeRangeClass == null)
            {
                return NotFound();
            }

            ReservationTimeRangeClassDto reservationTimeRangeClassDto = new ReservationTimeRangeClassDto
            {
                Id = reservationTimeRangeClass.Id,
                RemainingStudentsQuantity = reservationTimeRangeClass.RemainingStudentsQuantity,
                Class = _context.Classes.FirstOrDefault(c => c.Id == reservationTimeRangeClass.ClassId).Name,
                StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                EndTimeOnly = reservationTimeRangeClass.EndTimeOnly,
            };

            return View(reservationTimeRangeClassDto);
        }

        // GET: ReservationTimeRangeClasses/Create
        public IActionResult Create()
        {
            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name");

            return View();
        }

        // POST: ReservationTimeRangeClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationTimeRangeClassViewModel reservationTimeRangeClassViewModel)
        {
            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(reservationTimeRangeClassViewModel.StartDateOnly, reservationTimeRangeClassViewModel.EndDateOnly, reservationTimeRangeClassViewModel.StartTimeOnly, reservationTimeRangeClassViewModel.EndTimeOnly, reservationTimeRangeClassViewModel.RemainingStudentsQuantity, reservationTimeRangeClassViewModel.Class);
            if (ModelState.IsValid)
            {
                _context.Add(reservationTimeRangeClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservationTimeRangeClassViewModel);
        }

        // GET: ReservationTimeRangeClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationTimeRangeClass = await _context.ReservationTimeRangeClasses.FindAsync(id);
            if (reservationTimeRangeClass == null)
            {
                return NotFound();
            }

            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name");

            ReservationTimeRangeClassViewModel reservationTimeRangeClassViewModel = new ReservationTimeRangeClassViewModel
            {
                Id = reservationTimeRangeClass.Id,
                RemainingStudentsQuantity = reservationTimeRangeClass.RemainingStudentsQuantity,
                Class = reservationTimeRangeClass.ClassId,
                StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                EndTimeOnly = reservationTimeRangeClass.EndTimeOnly,
            };

            return View(reservationTimeRangeClassViewModel);
        }

        // POST: ReservationTimeRangeClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReservationTimeRangeClassViewModel reservationTimeRangeClassViewModel)
        {
            ReservationTimeRangeClass? reservationTimeRangeClass = _context.ReservationTimeRangeClasses.FirstOrDefault(reservation => reservation.Id == id);

            if (reservationTimeRangeClass == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    reservationTimeRangeClass.Update(reservationTimeRangeClassViewModel.StartDateOnly, reservationTimeRangeClassViewModel.EndDateOnly, reservationTimeRangeClassViewModel.StartTimeOnly, reservationTimeRangeClassViewModel.EndTimeOnly, reservationTimeRangeClassViewModel.RemainingStudentsQuantity, reservationTimeRangeClassViewModel.Class);

                    _context.Update(reservationTimeRangeClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationTimeRangeClassExists(reservationTimeRangeClass.Id))
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
            return View(reservationTimeRangeClassViewModel);
        }

        // GET: ReservationTimeRangeClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationTimeRangeClass = await _context.ReservationTimeRangeClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationTimeRangeClass == null)
            {
                return NotFound();
            }

            ReservationTimeRangeClassDto reservationTimeRangeClassDto = new ReservationTimeRangeClassDto
            {
                Id = reservationTimeRangeClass.Id,
                RemainingStudentsQuantity = reservationTimeRangeClass.RemainingStudentsQuantity,
                Class = _context.Classes.FirstOrDefault(c => c.Id == reservationTimeRangeClass.ClassId).Name,
                StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                EndTimeOnly = reservationTimeRangeClass.EndTimeOnly,
            };

            return View(reservationTimeRangeClassDto);
        }

        // POST: ReservationTimeRangeClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservationTimeRangeClass = await _context.ReservationTimeRangeClasses.FindAsync(id);
            if (reservationTimeRangeClass != null)
            {
                _context.ReservationTimeRangeClasses.Remove(reservationTimeRangeClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationTimeRangeClassExists(int id)
        {
            return _context.ReservationTimeRangeClasses.Any(e => e.Id == id);
        }
    }
}
