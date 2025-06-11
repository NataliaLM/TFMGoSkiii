using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Exceptions;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public class ReservationTimeRangeClassService : IReservationTimeRangeClassService
    {
        private readonly TFMGoSkiDbContext _context;

        public ReservationTimeRangeClassService(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReservationTimeRangeClassDto>> GetAllAsync()
        {
            var reservations = await _context.ReservationTimeRangeClasses.ToListAsync();
            var dtos = new List<ReservationTimeRangeClassDto>();

            foreach (var r in reservations)
            {
                var @class = await _context.Classes.FindAsync(r.ClassId);
                dtos.Add(new ReservationTimeRangeClassDto
                {
                    Id = r.Id,
                    RemainingStudentsQuantity = r.RemainingStudentsQuantity,
                    Class = @class?.Name ?? "(Unknown)",
                    StartDateOnly = r.StartDateOnly,
                    EndDateOnly = r.EndDateOnly,
                    StartTimeOnly = r.StartTimeOnly,
                    EndTimeOnly = r.EndTimeOnly
                });
            }

            return dtos;
        }

        public async Task<ReservationTimeRangeClassDto?> GetByIdAsync(int id)
        {
            var reservation = await _context.ReservationTimeRangeClasses.FindAsync(id);
            if (reservation == null) return null;

            var @class = await _context.Classes.FindAsync(reservation.ClassId);

            return new ReservationTimeRangeClassDto
            {
                Id = reservation.Id,
                RemainingStudentsQuantity = reservation.RemainingStudentsQuantity,
                Class = @class?.Name ?? "(Unknown)",
                StartDateOnly = reservation.StartDateOnly,
                EndDateOnly = reservation.EndDateOnly,
                StartTimeOnly = reservation.StartTimeOnly,
                EndTimeOnly = reservation.EndTimeOnly
            };
        }

        public async Task<ReservationTimeRangeClassViewModel?> GetEditViewModelAsync(int id)
        {
            var reservation = await _context.ReservationTimeRangeClasses.FindAsync(id);
            if (reservation == null) return null;

            return new ReservationTimeRangeClassViewModel
            {
                Id = reservation.Id,
                //RemainingStudentsQuantity = reservation.RemainingStudentsQuantity,
                Class = reservation.ClassId,
                StartDateOnly = reservation.StartDateOnly,
                EndDateOnly = reservation.EndDateOnly,
                StartTimeOnly = reservation.StartTimeOnly,
                EndTimeOnly = reservation.EndTimeOnly
            };
        }

        public async Task<UpdateReservationResult> CreateAsync(ReservationTimeRangeClassViewModel model)
        {
            #region No solapar fechas y horas de clases según intructor

            Class? @class = await _context.Classes.FindAsync(model.Class);
            if (@class == null) return UpdateReservationResult.Fail("Clase no encontrada");

            if (@class.InstructorId == 0) return UpdateReservationResult.Fail("La clase no tiene instructor");

            var classIds = await _context.Classes
                .Where(c => c.InstructorId == @class.InstructorId)
                .Select(c => c.Id)
                .ToListAsync();
            
            var reservations = await _context.ReservationTimeRangeClasses
                .Where(r => classIds.Contains(r.ClassId))
                .ToListAsync();
            
            foreach (var r in reservations)
            {
                bool datesOverlap = model.StartDateOnly <= r.EndDateOnly && model.EndDateOnly >= r.StartDateOnly;
                bool timesOverlap = model.StartTimeOnly < r.EndTimeOnly && model.EndTimeOnly > r.StartTimeOnly;

                if (datesOverlap && timesOverlap)
                {
                    return UpdateReservationResult.Fail("Ya existe una reserva que se solapa con este horario.");
                }
            }

            #endregion

            var reservation = new ReservationTimeRangeClass( //TODO: (Natalia) ¡Cuidado! Se tienen que ir restando reminingstudentsquantity después de cada reserva.
                model.StartDateOnly, model.EndDateOnly,
                model.StartTimeOnly, model.EndTimeOnly,
                @class.StudentQuantity, //model.RemainingStudentsQuantity,
                model.Class
            );

            _context.ReservationTimeRangeClasses.Add(reservation);
            await _context.SaveChangesAsync();

            return UpdateReservationResult.Ok();
        }

        public async Task<UpdateReservationResult> UpdateAsync(int id, ReservationTimeRangeClassViewModel model)
        {
            var @class = await _context.Classes.FindAsync(model.Class);
            if (@class == null) return UpdateReservationResult.Fail("Clase no encontrada");
            if (@class.InstructorId == 0) return UpdateReservationResult.Fail("La clase no tiene instructor");

            var classIds = await _context.Classes
                .Where(c => c.InstructorId == @class.InstructorId)
                .Select(c => c.Id)
                .ToListAsync();

            var reservations = await _context.ReservationTimeRangeClasses
                .Where(r => classIds.Contains(r.ClassId) && r.Id != id)
                .ToListAsync();

            foreach (var r in reservations)
            {
                bool datesOverlap = model.StartDateOnly <= r.EndDateOnly && model.EndDateOnly >= r.StartDateOnly;
                bool timesOverlap = model.StartTimeOnly < r.EndTimeOnly && model.EndTimeOnly > r.StartTimeOnly;

                if (datesOverlap && timesOverlap)
                {
                    return UpdateReservationResult.Fail("Ya existe una reserva que se solapa con este horario.");
                }
            }

            var reservation = await _context.ReservationTimeRangeClasses.FindAsync(id);
            if (reservation == null) return UpdateReservationResult.Fail("Reserva no encontrada");

            reservation.Update(
                model.StartDateOnly, model.EndDateOnly,
                model.StartTimeOnly, model.EndTimeOnly,
                @class.StudentQuantity,
                model.Class
            );

            _context.Update(reservation);
            await _context.SaveChangesAsync();

            return UpdateReservationResult.Ok();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var reservation = await _context.ReservationTimeRangeClasses.FindAsync(id);
            if (reservation == null) return false;

            _context.ReservationTimeRangeClasses.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        } //TODO: (Natalia) Hay que sumar Remaining Students si cancela la reserva.

        public bool Exists(int id)
        {
            return _context.ReservationTimeRangeClasses.Any(e => e.Id == id);
        }

        // Método para obtener SelectList para las clases (evitar la lógica en el controlador)
        public async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetClassSelectListAsync() //TODO: (Natalia) ¿qué es selectlist? Revisar todos los controllers.
        {
            var classes = await _context.Classes
                .Select(c => new
                {
                    Id = c.Id,
                    Description = c.Name + " (" + c.StudentQuantity + " students)"
                })
                .ToListAsync();
            return new SelectList(classes, "Id", "Description");
        }
    }
}
