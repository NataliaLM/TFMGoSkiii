﻿using System;
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
            return View(await _context.ReservationTimeRangeClasses.ToListAsync());
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

            return View(reservationTimeRangeClass);
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
        public async Task<IActionResult> Create([Bind("StartTime,EndTime, RemainingStudentsQuantity, ClassId")] DateTime startTime, DateTime endTime, int remainingStudentsQuantity, int classId)
        {
            ReservationTimeRangeClass reservationTimeRangeClass = new ReservationTimeRangeClass(startTime, endTime, remainingStudentsQuantity, classId);
            if (ModelState.IsValid)
            {
                _context.Add(reservationTimeRangeClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservationTimeRangeClass);
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

            return View(reservationTimeRangeClass);
        }

        // POST: ReservationTimeRangeClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StartTime,EndTime,RemainingStudentsQuantity,ClassId")] DateTime startTime, DateTime endTime, int remainingStudentsQuantity, int classId)
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
                    reservationTimeRangeClass.Update(startTime, endTime, remainingStudentsQuantity, classId);

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
            return View(reservationTimeRangeClass);
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

            return View(reservationTimeRangeClass);
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
