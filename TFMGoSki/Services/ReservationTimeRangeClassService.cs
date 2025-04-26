using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
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
                RemainingStudentsQuantity = reservation.RemainingStudentsQuantity,
                Class = reservation.ClassId,
                StartDateOnly = reservation.StartDateOnly,
                EndDateOnly = reservation.EndDateOnly,
                StartTimeOnly = reservation.StartTimeOnly,
                EndTimeOnly = reservation.EndTimeOnly
            };
        }

        public async Task CreateAsync(ReservationTimeRangeClassViewModel model)
        {
            var reservation = new ReservationTimeRangeClass(
                model.StartDateOnly, model.EndDateOnly,
                model.StartTimeOnly, model.EndTimeOnly,
                model.RemainingStudentsQuantity,
                model.Class
            );

            _context.ReservationTimeRangeClasses.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, ReservationTimeRangeClassViewModel model)
        {
            var reservation = await _context.ReservationTimeRangeClasses.FindAsync(id);
            if (reservation == null) return false;

            reservation.Update(
                model.StartDateOnly, model.EndDateOnly,
                model.StartTimeOnly, model.EndTimeOnly,
                model.RemainingStudentsQuantity,
                model.Class
            );

            _context.Update(reservation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var reservation = await _context.ReservationTimeRangeClasses.FindAsync(id);
            if (reservation == null) return false;

            _context.ReservationTimeRangeClasses.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.ReservationTimeRangeClasses.Any(e => e.Id == id);
        }

        // Método para obtener SelectList para las clases (evitar la lógica en el controlador)
        public async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetClassSelectListAsync() //TODO: (Natalia) ¿qué es selectlist? Revisar todos los controllers.
        {
            var classes = await _context.Classes.ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(classes, "Id", "Name");
        }
    }
}
