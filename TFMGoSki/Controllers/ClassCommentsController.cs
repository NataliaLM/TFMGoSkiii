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
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    [Authorize(Roles = "Admin,Worker,Client")]
    public class ClassCommentsController : Controller
    {
        private readonly TFMGoSkiDbContext _context;
        private readonly UserManager<User> _userManager;

        public ClassCommentsController(TFMGoSkiDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ClassComments
        public async Task<IActionResult> Index()
        {
            List<ClassCommentDto> classCommentDtos = new List<ClassCommentDto>();

            var comments = await _context.ClassComments.ToListAsync(); // Materializar la consulta

            foreach (var classComment in comments)
            {
                var classReservation = await _context.ClassReservations
                    .FirstOrDefaultAsync(c => c.Id == classComment.ClassReservationId);

                if (classReservation == null)
                    continue;

                var @class = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);

                if (@class == null)
                    continue;

                classCommentDtos.Add(new ClassCommentDto
                {
                    Id = classComment.Id,
                    ClassReservationName = @class.Name,
                    Text = classComment.Text,
                    Raiting = classComment.Raiting
                });
            }

            return View(classCommentDtos);
        }
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> IndexUser()
        {
            List<ClassCommentDto> classCommentDtos = new List<ClassCommentDto>();

            var comments = await _context.ClassComments.ToListAsync(); // Materializar la consulta

            var userId = _userManager.GetUserId(User);

            var classReservationsUser = _context.ClassReservations.Where(c => c.UserId.ToString().Equals(userId));

            foreach (var classComment in comments)
            {
                var classReservation = await _context.ClassReservations
                    .FirstOrDefaultAsync(c => c.Id == classComment.ClassReservationId);

                if (classReservation == null)
                    continue;

                var @class = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);

                if (@class == null)
                    continue;

                classCommentDtos.Add(new ClassCommentDto
                {
                    Id = classComment.Id,
                    ClassReservationName = @class.Name,
                    Text = classComment.Text,
                    Raiting = classComment.Raiting
                });
            }

            return View(classCommentDtos);
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

            ClassReservation classReservation = _context.ClassReservations.FirstOrDefault(c => c.Id == classComment.ClassReservationId);
            Class @class = _context.Classes.FirstOrDefault(c => c.Id == classReservation.ClassId);

            if (@class == null)
            {
                return NotFound();
            }

            ClassCommentDto classCommentDto = new ClassCommentDto()
            {
                ClassReservationName = @class.Name,
                Text = classComment.Text,
                Raiting = classComment.Raiting
            };

            if (User.IsInRole("Client"))
            {
                ViewData["FromDetails"] = true;
            }
            else
            {
                ViewData["FromDetails"] = false;
            }

            return View(classCommentDto);
        }

        // GET: ClassComments/Create
        public async Task<IActionResult> Create(int? classReservationId = null)
        {
            if (classReservationId == null)
            {
                var dbSetClassReservation = await _context.ClassReservations.ToListAsync();

                // Obtener el ID del rol "Client"
                var clientRoleId = await _context.Roles
                    .Where(r => r.Name == "Client")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                // Obtener los IDs de los usuarios que tienen el rol "Client"
                var clientUserIds = await _context.UserRoles
                    .Where(ur => ur.RoleId == clientRoleId)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                // Filtrar reservas de usuarios que son clientes
                var classReservations = dbSetClassReservation
                    .Where(d => clientUserIds.Contains(d.UserId))
                    .ToList();

                var listaUserClase = new List<KeyValuePair<int, string>>();

                foreach (var classReservation in classReservations)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == classReservation.UserId);
                    var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);

                    if (user != null && @class != null)
                    {
                        string displayText = $"{user.UserName} - {@class.Name}";
                        listaUserClase.Add(new KeyValuePair<int, string>(classReservation.Id, displayText));
                    }
                }

                ViewBag.ClassReservationId = new SelectList(listaUserClase, "Key", "Value");

                ViewData["FromDetails"] = false;

                return View(new ClassCommentViewModel());
            }
            else
            {
                var reservation = await _context.ClassReservations.FirstOrDefaultAsync(r => r.Id == classReservationId);
                if (reservation == null)
                    return NotFound();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == reservation.UserId);
                var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == reservation.ClassId);
                var reservationTimeRangeClass = _context.ReservationTimeRangeClasses.FirstOrDefault(r => r.Id == reservation.ReservationTimeRangeClassId);

                if (user == null || @class == null || reservationTimeRangeClass == null)
                    return NotFound();

                ViewBag.DisplayName = $"{user.UserName} - {@class.Name}";

                var viewModel = new ClassCommentViewModel
                {
                    ClassReservationId = reservation.Id,
                    ClassReservationName = $"{@class.Name} - {reservationTimeRangeClass.StartDateOnly} - {reservationTimeRangeClass.EndDateOnly}"
                };

                ViewData["FromDetails"] = true;

                return View(viewModel);
            }
        }

        // POST: ClassComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(int classReservationId, ClassCommentViewModel classCommentViewModel)
        {
            // Verificar si ya existe un comentario para esa reserva
            var existingComment = await _context.ClassComments
                .FirstOrDefaultAsync(c => c.ClassReservationId == classReservationId);

            if (existingComment != null)
            {
                // Ya existe un comentario, mostrar advertencia en la vista
                ModelState.AddModelError(string.Empty, "A comment already exists for this reservation.");
                ViewData["FromDetails"] = true;
                // En cualquier punto donde retornes la vista:
                var reservation = await _context.ClassReservations.FirstOrDefaultAsync(r => r.Id == classReservationId);

                classCommentViewModel.ClassReservationName = GenerarNombreDeClase(reservation);

                return View(classCommentViewModel);
            }
            ViewData["FromDetails"] = false;
            ClassComment classComment = new ClassComment(classReservationId, classCommentViewModel.Text, classCommentViewModel.Raiting);
            if (ModelState.IsValid)
            {
                _context.Add(classComment);
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
            if (User.IsInRole("Client"))
            {
                ViewData["IsClient"] = true;
            }
            else
            {
                ViewData["IsClient"] = false;
            }
            return View(classCommentViewModel);
        }

        private string GenerarNombreDeClase(ClassReservation reservation)
        {
            var @class = _context.Classes.First(c => c.Id == reservation.ClassId);
            var range = _context.ReservationTimeRangeClasses.First(r => r.Id == reservation.ReservationTimeRangeClassId);
            return $"{@class.Name} - {range.StartDateOnly} - {range.EndDateOnly}";
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

            //var classReservations = await _context.ClassReservations.ToListAsync();
            //var listaClienteClase = new List<KeyValuePair<int, string>>();

            //foreach (var classReservation in classReservations)
            //{
            //    var client = await _context.Users.FirstOrDefaultAsync(c => c.Id == classReservation.UserId);
            //    var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);

            //    if (client != null && @class != null)
            //    {
            //        string displayText = $"{client.UserName} - {@class.Name}";
            //        listaClienteClase.Add(new KeyValuePair<int, string>(classReservation.Id, displayText));
            //    }
            //}

            //ViewBag.ClassReservationId = new SelectList(listaClienteClase, "Key", "Value", classComment.ClassReservationId);

            var classReservation = await _context.ClassReservations.FirstOrDefaultAsync(cr => cr.Id == classComment.ClassReservationId);

            var client = await _context.Users.FirstOrDefaultAsync(c => c.Id == classReservation.UserId);
            var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);

            string displayText = client != null && @class != null ? $"{client.UserName} - {@class.Name}" : "N/A";

            ClassCommentViewModel classCommentViewModel = new ClassCommentViewModel()
            {
                Id = classComment.Id,
                ClassReservationId = classComment.ClassReservationId,
                ClassReservationName = displayText,
                Text = classComment.Text,
                Raiting = classComment.Raiting
            };

            return View(classCommentViewModel);
        }

        // POST: ClassComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ClassCommentViewModel classCommentViewModel)
        {
            ClassComment? classComment = _context.ClassComments.FirstOrDefault(c => c.Id == id);

            if (ModelState.IsValid && classComment != null)
            {
                try
                {
                    classComment.Update(classCommentViewModel.ClassReservationId, classCommentViewModel.Text, classCommentViewModel.Raiting);
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
                if (User.IsInRole("Client"))
                {
                    return RedirectToAction(nameof(IndexUser));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            //#region classReservation
            //var classReservations = await _context.ClassReservations.ToListAsync();
            //var listaClienteClase = new List<KeyValuePair<int, string>>();

            //foreach (var classReservation in classReservations)
            //{
            //    var client = await _context.Users.FirstOrDefaultAsync(c => c.Id == classReservation.UserId);
            //    var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classReservation.ClassId);

            //    if (client != null && @class != null)
            //    {
            //        string displayText = $"{client.UserName} - {@class.Name}";
            //        listaClienteClase.Add(new KeyValuePair<int, string>(classReservation.Id, displayText));
            //    }
            //}

            //ViewBag.ClassReservationId = new SelectList(listaClienteClase, "Key", "Value", classComment.ClassReservationId);
            //#endregion

            return View(classCommentViewModel);
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

            ClassReservation classReservation = _context.ClassReservations.FirstOrDefault(c => c.Id == classComment.ClassReservationId);
            Class @class = _context.Classes.FirstOrDefault(c => c.Id == classReservation.ClassId);

            if (@class == null)
            {
                return NotFound();
            }

            ClassCommentDto classCommentDto = new ClassCommentDto()
            {
                ClassReservationName = @class.Name,
                Text = classComment.Text,
                Raiting = classComment.Raiting
            };

            return View(classCommentDto);
        }

        // POST: ClassComments/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classComment = await _context.ClassComments.FindAsync(id);
            if (classComment != null)
            {
                _context.ClassComments.Remove(classComment);
            }

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

        private bool ClassCommentExists(int id)
        {
            return _context.ClassComments.Any(e => e.Id == id);
        }
    }
}
