using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Exceptions;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class ClassReservationsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;
        private readonly UserManager<User> _userManager;
        public ClassReservationsController(TFMGoSkiDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                var reservationTimeRangeClass = await _context.ReservationTimeRangeClasses.FirstOrDefaultAsync(r => r.Id == reservation.ReservationTimeRangeClassId);

                if (client == null)
                {
                    return NotFound();
                }
                if (client.UserName == null)
                {
                    return NotFound();
                }
                if (@class == null)
                {
                    return NotFound();
                }
                if (reservationTimeRangeClass == null)
                {
                    return NotFound();
                }

                if (client != null && @class != null)
                {
                    listClassReservationDto.Add(new ClassReservationDto
                    {
                        Id = reservation.Id,
                        ClientName = client.UserName,
                        ClassName = @class.Name,
                        ReservationTimeRangeClassDto = new ReservationTimeRangeClassDto
                        {
                            StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                            EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                            StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                            EndTimeOnly = reservationTimeRangeClass.EndTimeOnly
                        }
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
            var client = await _context.Users.FirstOrDefaultAsync(c => c.Id == classReservation.UserId);
            var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);
            var reservationTimeRangeClass = await _context.ReservationTimeRangeClasses.FirstOrDefaultAsync(r => r.Id == classReservation.ReservationTimeRangeClassId);

            ClassReservationDto classReservationDto = new ClassReservationDto()
            {
                ClientName = client.UserName,
                ClassName = @class.Name,
                ReservationTimeRangeClassDto = new ReservationTimeRangeClassDto
                {
                    StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                    EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                    StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                    EndTimeOnly = reservationTimeRangeClass.EndTimeOnly
                }
            };

            return View(classReservationDto);
        }

        // GET: ClassReservations/Create
        public async Task<IActionResult> Create(int? classId = null, int? reservationTimeRangeClassId = null)
        {
            if(classId != null && reservationTimeRangeClassId != null)
            {
                var userId = _userManager.GetUserId(User);
                var classItem = await _context.Classes.FindAsync(classId);
                var reservation = await _context.ReservationTimeRangeClasses.FindAsync(reservationTimeRangeClassId);
                if (classItem == null || reservation == null || userId == null)
                {
                    return NotFound();
                }
                var viewModel = new ClassReservationViewModel
                {
                    ClassId = classId.Value,
                    ReservationTimeRangeClassId = reservationTimeRangeClassId.Value,
                    UserId = int.Parse(userId),
                    ClassName = classItem.Name,
                    ReservationTimeRangeClassName = $"[{reservation.StartDateOnly} - {reservation.EndDateOnly}] [{reservation.StartTimeOnly} - {reservation.EndTimeOnly}]"
                };
                ViewData["FromDetails"] = true;
                return View(viewModel);
            }
            else
            {
                ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName");
                ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name");
                ViewData["FromDetails"] = false;
                return View();
            }                
        }

        // POST: ClassReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(ClassReservationViewModel classReservationViewModel)
        {
            ClassReservation classReservation = new ClassReservation(classReservationViewModel.UserId, classReservationViewModel.ClassId, classReservationViewModel.ReservationTimeRangeClassId);

            if (ModelState.IsValid)
            {
                #region Remaining Students Quantity

                ReservationTimeRangeClass? reservationTimeRangeClass = _context.ReservationTimeRangeClasses.FirstOrDefault(r => r.Id == classReservation.ReservationTimeRangeClassId);
                Class? @class = _context.Classes.FirstOrDefault(c => c.Id == classReservationViewModel.ClassId);

                if(reservationTimeRangeClass == null)
                {
                    return View(classReservationViewModel);
                }

                if (@class == null)
                {
                    return View(classReservationViewModel);
                }

                if (reservationTimeRangeClass.RemainingStudentsQuantity == -1)
                {
                    ModelState.AddModelError(string.Empty, "There are no places left to book in the class.");
                }
                if (reservationTimeRangeClass.RemainingStudentsQuantity == 0)
                {
                    reservationTimeRangeClass.RemainingStudentsQuantity = @class.StudentQuantity;
                }
                else
                {
                    reservationTimeRangeClass.RemainingStudentsQuantity += -1;
                    if (reservationTimeRangeClass.RemainingStudentsQuantity == 0)
                    {
                        reservationTimeRangeClass.RemainingStudentsQuantity = -1;
                    }
                }

                _context.Update(reservationTimeRangeClass);
                await _context.SaveChangesAsync();

                #endregion
            
                _context.Add(classReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", classReservationViewModel.UserId);
            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);
            return View(classReservationViewModel);
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

            ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name");

            ClassReservationViewModel classReservationViewModel = new ClassReservationViewModel()
            {
                Id = classReservation.Id,
                UserId = classReservation.UserId,
                ClassId = classReservation.ClassId,
                ReservationTimeRangeClassId = classReservation.ReservationTimeRangeClassId
            };

            #region desplegable reservation time range

            var timeRanges = _context.ReservationTimeRangeClasses
                .Where(r => r.ClassId == classReservation.ClassId)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"{r.StartDateOnly:dd/MM/yyyy} {r.StartTimeOnly:hh\\:mm} - {r.EndDateOnly:dd/MM/yyyy} {r.EndTimeOnly:hh\\:mm}"
                }).ToList();

            ViewBag.TimeRanges = timeRanges;

            #endregion

            return View(classReservationViewModel);
        }

        // POST: ClassReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ClassReservationViewModel classReservationViewModel)
        {
            ClassReservation? classReservation = _context.ClassReservations.FirstOrDefault(c => c.Id == classReservationViewModel.Id);

            if(classReservation == null)
            {
                return View(classReservationViewModel);
            }

            #region Remaining Students Quantity

            ReservationTimeRangeClass? reservationTimeRangeClass = _context.ReservationTimeRangeClasses.FirstOrDefault(r => r.Id == classReservation.ReservationTimeRangeClassId);
            Class? @class = _context.Classes.FirstOrDefault(c => c.Id == classReservationViewModel.ClassId);

            if (reservationTimeRangeClass == null)
            {
                return View(classReservationViewModel);
            }

            if (@class == null)
            {
                return View(classReservationViewModel);
            }

            var originalClassId = classReservation.ClassId;
            var originalTimeRangeId = classReservation.ReservationTimeRangeClassId;

            if (originalTimeRangeId != classReservationViewModel.ReservationTimeRangeClassId) //reserva con otro rango
            {
                if (reservationTimeRangeClass.RemainingStudentsQuantity == -1)
                {
                    ModelState.AddModelError(string.Empty, "There are no places left to book in the class.");
                }
                if (reservationTimeRangeClass.RemainingStudentsQuantity == 0)
                {
                    reservationTimeRangeClass.RemainingStudentsQuantity = @class.StudentQuantity;
                }
                else
                {
                    reservationTimeRangeClass.RemainingStudentsQuantity += -1;
                    if (reservationTimeRangeClass.RemainingStudentsQuantity == 0)
                    {
                        reservationTimeRangeClass.RemainingStudentsQuantity = -1;
                    }
                }

                _context.Update(reservationTimeRangeClass);
                await _context.SaveChangesAsync();

                Class? @classAnterior = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classReservation.Id);
                if(@classAnterior == null)
                {
                    return View(classReservationViewModel);
                }
                ReservationTimeRangeClass? reservationTimeRangeClassAnterior = await _context.ReservationTimeRangeClasses.FirstOrDefaultAsync(r => r.ClassId == @classAnterior.Id);
                if (reservationTimeRangeClassAnterior == null)
                {
                    return View(classReservationViewModel);
                }
                if (reservationTimeRangeClassAnterior.RemainingStudentsQuantity == -1)
                {
                    reservationTimeRangeClassAnterior.RemainingStudentsQuantity = 1;
                }
                else
                {
                    reservationTimeRangeClassAnterior.RemainingStudentsQuantity += 1;
                }

                _context.Update(reservationTimeRangeClass);
                await _context.SaveChangesAsync();
            }

            #endregion

            if (ModelState.IsValid && classReservation != null)
            {
                try
                {
                    classReservation.Update(classReservationViewModel.UserId, classReservationViewModel.ClassId, classReservationViewModel.ReservationTimeRangeClassId);
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
            ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", classReservationViewModel.UserId);
            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);
            return View(classReservationViewModel);
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

            var client = await _context.Users.FirstOrDefaultAsync(c => c.Id == classReservation.UserId);
            var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);
            var reservationTimeRangeClass = await _context.ReservationTimeRangeClasses.FirstOrDefaultAsync(r => r.Id == classReservation.ReservationTimeRangeClassId);

            if (client == null)
            {
                return NotFound();
            }
            if (client.FullName == null)
            {
                return NotFound();
            }
            if (@class == null)
            {
                return NotFound();
            }
            if (reservationTimeRangeClass == null)
            {
                return NotFound();
            }

            ClassReservationDto classReservationDto = new ClassReservationDto()
            {
                Id = classReservation.Id,
                ClientName = client.FullName,
                ClassName = @class.Name,
                ReservationTimeRangeClassDto = new ReservationTimeRangeClassDto
                {
                    StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                    EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                    StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                    EndTimeOnly = reservationTimeRangeClass.EndTimeOnly
                }
            };

            return View(classReservationDto);
        }

        // POST: ClassReservations/Delete/5
        [HttpPost, ActionName("Delete")]
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

        [HttpGet]
        public async Task<IActionResult> GetTimeRangesByClassId(int classId)
        {
            var timeRanges = await _context.ReservationTimeRangeClasses
                .Where(r => r.ClassId == classId)
                .Select(r => new
                {
                    r.Id,
                    Display = $"{r.StartDateOnly:dd/MM/yyyy} {r.StartTimeOnly:hh\\:mm} - {r.EndDateOnly:dd/MM/yyyy} {r.EndTimeOnly:hh\\:mm}"
                })
                .ToListAsync();

            return Json(timeRanges);
        }
    }
}
