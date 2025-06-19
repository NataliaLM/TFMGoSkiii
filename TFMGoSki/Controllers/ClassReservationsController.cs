using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin,Worker,Client")]
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
                        NumberPersonsBooked = reservation.NumberPersonsBooked,
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
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> IndexUser()
        {
            var classReservations = await _context.ClassReservations.ToListAsync();

            var userId = _userManager.GetUserId(User);

            var classReservationsUser = classReservations.Where(c => c.UserId.ToString().Equals(userId));

            var listClassReservationDto = new List<ClassReservationDto>();

            foreach (var reservation in classReservationsUser)
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
                        //ClientName = client.UserName,
                        ClassName = @class.Name,
                        NumberPersonsBooked = reservation.NumberPersonsBooked,
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
                Id = classReservation.Id,
                ClientName = client.UserName,
                ClassName = @class.Name,
                NumberPersonsBooked = classReservation.NumberPersonsBooked,
                ReservationTimeRangeClassDto = new ReservationTimeRangeClassDto
                {
                    StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                    EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                    StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                    EndTimeOnly = reservationTimeRangeClass.EndTimeOnly
                }
            };

            if (User.IsInRole("Client"))
            {
                ViewData["FromDetails"] = true;
            }
            else
            {
                ViewData["FromDetails"] = false;
            }

            return View(classReservationDto);
        }

        // GET: ClassReservations/Create
        public async Task<IActionResult> Create(int? classId = null, int? reservationTimeRangeClassId = null)
        {
            if (classId != null && reservationTimeRangeClassId != null && User.IsInRole("Client"))
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
                // Obtener el ID del rol "Client"
                var clientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Client");
                if (clientRole == null)
                {
                    return NotFound("The role 'Client' was not found.");
                }

                // Obtener los IDs de usuarios que tienen el rol "Client"
                var clientUserIds = await _context.UserRoles
                    .Where(ur => ur.RoleId == clientRole.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                // Obtener los usuarios con ese rol
                var clientUsers = await _context.Users
                    .Where(u => clientUserIds.Contains(u.Id))
                    .ToListAsync();

                ViewBag.UserId = new SelectList(clientUsers, "Id", "UserName");
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
            ClassReservation classReservation = new ClassReservation(
                classReservationViewModel.UserId,
                classReservationViewModel.ClassId,
                classReservationViewModel.ReservationTimeRangeClassId,
                classReservationViewModel.NumberPersonsBooked
            );

            if (ModelState.IsValid)
            {
                #region Remaining Students Quantity

                var reservationTimeRangeClass = _context.ReservationTimeRangeClasses
                    .FirstOrDefault(r => r.Id == classReservation.ReservationTimeRangeClassId);
                var @class = _context.Classes
                    .FirstOrDefault(c => c.Id == classReservationViewModel.ClassId);

                if (reservationTimeRangeClass == null || @class == null)
                {
                    ModelState.AddModelError(string.Empty, "Reservation time range or class not found.");
                    return View(classReservationViewModel);
                }

                int numberToBook = classReservationViewModel.NumberPersonsBooked;

                // Si ya está marcado como sin plazas (-1)
                if (reservationTimeRangeClass.RemainingStudentsQuantity == -1)
                {
                    ModelState.AddModelError(string.Empty, "There are no places left to book in the class.");
                    ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", classReservationViewModel.UserId);
                    ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);
                    return View(classReservationViewModel);
                }

                // Si aún no se ha inicializado el número de plazas disponibles
                if (reservationTimeRangeClass.RemainingStudentsQuantity == 0)
                {
                    reservationTimeRangeClass.RemainingStudentsQuantity = @class.StudentQuantity;
                }

                // Verificamos que haya suficientes plazas disponibles
                if (reservationTimeRangeClass.RemainingStudentsQuantity < numberToBook)
                {
                    ModelState.AddModelError(string.Empty, $"Only {reservationTimeRangeClass.RemainingStudentsQuantity} places are available.");
                    ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", classReservationViewModel.UserId);
                    ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);
                    return View(classReservationViewModel);
                }

                // Restamos las plazas reservadas
                reservationTimeRangeClass.RemainingStudentsQuantity -= numberToBook;

                // Si ya no quedan plazas, marcamos con -1
                if (reservationTimeRangeClass.RemainingStudentsQuantity == 0)
                {
                    reservationTimeRangeClass.RemainingStudentsQuantity = -1;
                }

                _context.Update(reservationTimeRangeClass);
                await _context.SaveChangesAsync();

                #endregion

                // Guardamos la reserva
                _context.Add(classReservation);
                await _context.SaveChangesAsync();

                if (User.IsInRole("Client"))
                {
                    return RedirectToAction(nameof(IndexUser));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
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

            // Obtener el ID del rol "Client"
            var clientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Client");
            if (clientRole == null)
            {
                return NotFound("The role 'Client' was not found.");
            }

            // Obtener los IDs de usuarios que tienen el rol "Client"
            var clientUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == clientRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Obtener los usuarios con ese rol
            var clientUsers = await _context.Users
                .Where(u => clientUserIds.Contains(u.Id))
                .ToListAsync();

            ViewBag.UserId = new SelectList(clientUsers, "Id", "UserName");

            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name");

            var userId = _userManager.GetUserId(User);
            var classItem = await _context.Classes.FindAsync(classReservation.ClassId);
            var reservation = await _context.ReservationTimeRangeClasses.FindAsync(classReservation.ReservationTimeRangeClassId);
            if (classItem == null || reservation == null || userId == null)
            {
                return NotFound();
            }
            var classReservationViewModel = new ClassReservationViewModel
            {
                Id = classReservation.Id,
                ClassId = classReservation.ClassId,
                ReservationTimeRangeClassId = classReservation.ReservationTimeRangeClassId,
                NumberPersonsBooked = classReservation.NumberPersonsBooked,
                UserId = int.Parse(userId),
                ClassName = classItem.Name,
                ReservationTimeRangeClassName = $"[{reservation.StartDateOnly} - {reservation.EndDateOnly}] [{reservation.StartTimeOnly} - {reservation.EndTimeOnly}]"
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

            if (User.IsInRole("Client"))
            {
                ViewData["FromDetails"] = true;
            }
            else
            {
                ViewData["FromDetails"] = false;
            }

            return View(classReservationViewModel);
        }

        // POST: ClassReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ClassReservationViewModel classReservationViewModel)
        {
            var classReservation = _context.ClassReservations.FirstOrDefault(c => c.Id == classReservationViewModel.Id);

            if (classReservation == null)
            {
                if (User.IsInRole("Client"))
                {
                    ViewData["FromDetails"] = true;
                }
                else
                {
                    ViewData["FromDetails"] = false;
                }
                return View(classReservationViewModel);
            }

            #region Remaining Students Quantity

            var originalTimeRangeId = classReservation.ReservationTimeRangeClassId;
            var originalClassId = classReservation.ClassId;
            var originalNumberBooked = classReservation.NumberPersonsBooked;

            var newTimeRangeId = classReservationViewModel.ReservationTimeRangeClassId;
            var newClassId = classReservationViewModel.ClassId;
            var newNumberBooked = classReservationViewModel.NumberPersonsBooked;

            var originalReservationTimeRange = await _context.ReservationTimeRangeClasses
                .FirstOrDefaultAsync(r => r.Id == originalTimeRangeId);
            var newReservationTimeRange = await _context.ReservationTimeRangeClasses
                .FirstOrDefaultAsync(r => r.Id == newTimeRangeId);

            var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == newClassId);

            if (@class == null || newReservationTimeRange == null || originalReservationTimeRange == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid class or reservation time range.");
                ViewBag.UserId = await GetReservasCliente(classReservationViewModel);

                ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);

                if (User.IsInRole("Client"))
                {
                    ViewData["FromDetails"] = true;
                }
                else
                {
                    ViewData["FromDetails"] = false;
                }

                return View(classReservationViewModel);
            }

            // Si se cambió el rango de horario
            if (originalTimeRangeId != newTimeRangeId)
            {
                // Verificar disponibilidad en el nuevo rango
                if (newReservationTimeRange.RemainingStudentsQuantity == -1)
                {
                    ModelState.AddModelError(string.Empty, "There are no places left to book in the class.");
                    ViewBag.UserId = await GetReservasCliente(classReservationViewModel);

                    ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);

                    if (User.IsInRole("Client"))
                    {
                        ViewData["FromDetails"] = true;
                    }
                    else
                    {
                        ViewData["FromDetails"] = false;
                    }

                    return View(classReservationViewModel);
                }

                if (newReservationTimeRange.RemainingStudentsQuantity == 0)
                {
                    newReservationTimeRange.RemainingStudentsQuantity = @class.StudentQuantity;
                }

                if (newReservationTimeRange.RemainingStudentsQuantity < newNumberBooked)
                {
                    ModelState.AddModelError(string.Empty, $"Only {newReservationTimeRange.RemainingStudentsQuantity} places available.");
                    ViewBag.UserId = await GetReservasCliente(classReservationViewModel);

                    ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);

                    if (User.IsInRole("Client"))
                    {
                        ViewData["FromDetails"] = true;
                    }
                    else
                    {
                        ViewData["FromDetails"] = false;
                    }

                    return View(classReservationViewModel);
                }

                // Restamos nuevas reservas
                newReservationTimeRange.RemainingStudentsQuantity -= newNumberBooked;
                if (newReservationTimeRange.RemainingStudentsQuantity == 0)
                {
                    newReservationTimeRange.RemainingStudentsQuantity = -1;
                }

                // Revertimos reservas originales
                if (originalReservationTimeRange.RemainingStudentsQuantity == -1)
                {
                    originalReservationTimeRange.RemainingStudentsQuantity = originalNumberBooked;
                }
                else
                {
                    originalReservationTimeRange.RemainingStudentsQuantity += originalNumberBooked;
                }

                _context.Update(originalReservationTimeRange);
                _context.Update(newReservationTimeRange);
                await _context.SaveChangesAsync();
            }
            // Si solo cambió la cantidad de personas pero el horario es el mismo
            else if (originalNumberBooked != newNumberBooked)
            {
                int difference = newNumberBooked - originalNumberBooked;

                if (difference > 0) // Quiere reservar más personas
                {
                    if (newReservationTimeRange.RemainingStudentsQuantity == -1)
                    {
                        ModelState.AddModelError(string.Empty, "There are no places left to book in the class.");
                        ViewBag.UserId = await GetReservasCliente(classReservationViewModel);

                        ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);

                        if (User.IsInRole("Client"))
                        {
                            ViewData["FromDetails"] = true;
                        }
                        else
                        {
                            ViewData["FromDetails"] = false;
                        }

                        return View(classReservationViewModel);
                    }

                    if (newReservationTimeRange.RemainingStudentsQuantity == 0)
                    {
                        newReservationTimeRange.RemainingStudentsQuantity = @class.StudentQuantity;
                    }

                    if (newReservationTimeRange.RemainingStudentsQuantity < difference)
                    {
                        ModelState.AddModelError(string.Empty, $"Only {newReservationTimeRange.RemainingStudentsQuantity} more places are available.");
                        ViewBag.UserId = await GetReservasCliente(classReservationViewModel);

                        ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);

                        if (User.IsInRole("Client"))
                        {
                            ViewData["FromDetails"] = true;
                        }
                        else
                        {
                            ViewData["FromDetails"] = false;
                        }

                        return View(classReservationViewModel);
                    }

                    newReservationTimeRange.RemainingStudentsQuantity -= difference;
                    if (newReservationTimeRange.RemainingStudentsQuantity == 0)
                    {
                        newReservationTimeRange.RemainingStudentsQuantity = -1;
                    }
                }
                else // Está reduciendo el número de personas
                {
                    if (newReservationTimeRange.RemainingStudentsQuantity == -1)
                    {
                        newReservationTimeRange.RemainingStudentsQuantity = -difference;//Ahora es positivo
                    }
                    else
                    {
                        newReservationTimeRange.RemainingStudentsQuantity += -difference;
                    }
                }

                _context.Update(newReservationTimeRange);
                await _context.SaveChangesAsync();
            }

            #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    classReservation.Update(
                        classReservationViewModel.UserId,
                        classReservationViewModel.ClassId,
                        classReservationViewModel.ReservationTimeRangeClassId,
                        classReservationViewModel.NumberPersonsBooked
                    );

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

            ViewBag.UserId = await GetReservasCliente(classReservationViewModel);
            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "Name", classReservationViewModel.ClassId);

            if (User.IsInRole("Client"))
            {
                ViewData["FromDetails"] = true;
            }
            else
            {
                ViewData["FromDetails"] = false;
            }

            return View(classReservationViewModel);
        }

        public async Task<dynamic> GetReservasCliente(ClassReservationViewModel classReservationViewModel)
        {
            // Obtener el ID del rol "Client"
            var clientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Client");
            if (clientRole == null)
            {
                return NotFound("The role 'Client' was not found.");
            }

            // Obtener los IDs de usuarios que tienen el rol "Client"
            var clientUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == clientRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Obtener los usuarios con ese rol
            var clientUsers = await _context.Users
                .Where(u => clientUserIds.Contains(u.Id))
                .ToListAsync();

            ViewBag.UserId = new SelectList(clientUsers, "Id", "UserName", classReservationViewModel.UserId);

            return ViewBag.UserId;
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
                ClientName = client.UserName,
                ClassName = @class.Name,
                NumberPersonsBooked = classReservation.NumberPersonsBooked,
                ReservationTimeRangeClassDto = new ReservationTimeRangeClassDto
                {
                    StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                    EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                    StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                    EndTimeOnly = reservationTimeRangeClass.EndTimeOnly
                }
            };

            if (User.IsInRole("Client"))
            {
                ViewData["FromDetails"] = true;
            }
            else
            {
                ViewData["FromDetails"] = false;
            }

            return View(classReservationDto);
        }

        // POST: ClassReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classComments = _context.ClassComments.Where(cc => cc.ClassReservationId == id).ToList();

            if (classComments.Any())
            {
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
                    Id = classReservation.Id,
                    ClientName = client.UserName,
                    ClassName = @class.Name,
                    NumberPersonsBooked = classReservation.NumberPersonsBooked,
                    ReservationTimeRangeClassDto = new ReservationTimeRangeClassDto
                    {
                        StartDateOnly = reservationTimeRangeClass.StartDateOnly,
                        EndDateOnly = reservationTimeRangeClass.EndDateOnly,
                        StartTimeOnly = reservationTimeRangeClass.StartTimeOnly,
                        EndTimeOnly = reservationTimeRangeClass.EndTimeOnly
                    }
                };

                // Agrega el error al modelo
                ModelState.AddModelError(string.Empty, "The class reservation cannot be deleted because it has associated a comment.");

                if (User.IsInRole("Client"))
                {
                    ViewData["FromDetails"] = true;
                }
                else
                {
                    ViewData["FromDetails"] = false;
                }

                // Devuelve la vista Delete con el modelo y el error
                return View("Delete", classReservationDto);
            }

            var classReservationDelete = await _context.ClassReservations.FindAsync(id);
            if (classReservationDelete != null)
            {
                _context.ClassReservations.Remove(classReservationDelete);
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
