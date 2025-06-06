using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Services
{
    public class ClassService : IClassService
    {
        private readonly TFMGoSkiDbContext _context;

        public ClassService(TFMGoSkiDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClassDto>> GetAllClassesAsync(bool? finalizadas = null)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);
            var classDtos = new List<ClassDto>();

            // Obtener todas las clases (filtraremos después)
            var classes = await _context.Classes.ToListAsync();

            foreach (var clase in classes)
            {
                var city = await _context.Cities.FindAsync(clase.CityId);
                var instructor = await _context.Instructors.FindAsync(clase.InstructorId);

                // Filtrar reservas por clase y por si están finalizadas o no
                var reservationsQuery = _context.ReservationTimeRangeClasses
                    .Where(r => r.ClassId == clase.Id);

                // Clases que ya comenzaron (StartDateOnly < hoy) se consideran no disponibles
                //reservationsQuery = reservationsQuery.Where(r => r.StartDateOnly >= now);

                if (finalizadas == true)
                {
                    reservationsQuery = reservationsQuery.Where(r => r.StartDateOnly < now || (r.StartDateOnly == now && r.StartTimeOnly < nowTime));
                }
                else if (finalizadas == false)
                {
                    reservationsQuery = reservationsQuery.Where(r => r.StartDateOnly >= now || (r.StartDateOnly == now && r.StartTimeOnly >= nowTime));
                }

                var reservationTimeRangeClassDtos = await reservationsQuery
                    .Select(r => new ReservationTimeRangeClassDto
                    {
                        RemainingStudentsQuantity = r.RemainingStudentsQuantity,
                        StartDateOnly = r.StartDateOnly,
                        EndDateOnly = r.EndDateOnly,
                        StartTimeOnly = r.StartTimeOnly,
                        EndTimeOnly = r.EndTimeOnly
                    })
                    .ToListAsync();

                // Si la clase no tiene reservas disponibles según el filtro, no la mostramos
                if (!reservationTimeRangeClassDtos.Any() && finalizadas != null) continue; //Si No hay reservas pero No se especifica si quiere las finalizadas/No finalizadas, se muestra igual.

                classDtos.Add(new ClassDto
                {
                    Id = clase.Id,
                    Name = clase.Name,
                    Price = clase.Price,
                    StudentQuantity = clase.StudentQuantity,
                    ClassLevel = clase.ClassLevel,
                    InstructorName = instructor?.Name,
                    CityName = city?.Name,
                    ReservationTimeRangeClassDto = reservationTimeRangeClassDtos
                });
            }

            return classDtos;
        }

        public async Task<ClassDto?> GetClassDetailsAsync(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return null;

            var city = await _context.Cities.FindAsync(@class.CityId);
            var instructor = await _context.Instructors.FindAsync(@class.InstructorId);

            //var reservationTimeRangeClass = await _context.ReservationTimeRangeClasses.FirstOrDefaultAsync(c => c.ClassId == @class.Id);

            #region ReservationTimeRangeClass
            List<ReservationTimeRangeClass> listReservationTimeRange = await _context.ReservationTimeRangeClasses
                .Where(c => c.ClassId == @class.Id)
                .ToListAsync();
            List<ReservationTimeRangeClassDto> reservationTimeRangeClassDtos = new List<ReservationTimeRangeClassDto>();
            foreach (var r in listReservationTimeRange)
            {
                reservationTimeRangeClassDtos.Add(new ReservationTimeRangeClassDto
                {
                    RemainingStudentsQuantity = r.RemainingStudentsQuantity,
                    StartDateOnly = r.StartDateOnly,
                    EndDateOnly = r.EndDateOnly,
                    StartTimeOnly = r.StartTimeOnly,
                    EndTimeOnly = r.EndTimeOnly
                });
            }
            #endregion

            return new ClassDto
            {
                Id = @class.Id,
                Name = @class.Name,
                Price = @class.Price,
                StudentQuantity = @class.StudentQuantity,
                ClassLevel = @class.ClassLevel,
                InstructorName = instructor?.Name,
                CityName = city?.Name,
                #region ReservationTimeRangeClass
                ReservationTimeRangeClassDto = reservationTimeRangeClassDtos
                #endregion
            };
        }

        public async Task<bool> CreateClassAsync(ClassViewModel model)
        {
            var classFound = _context.Classes.FirstOrDefault(c => c.Name.Equals(model.Name));

            if (classFound != null)
            {
                return false;
            }

            var @class = new Class(
                model.Name,
                model.Price!.Value,
                model.StudentQuantity!.Value,
                model.ClassLevel!.Value,
                model.Instructor!.Value,
                model.City!.Value
            );

            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ClassViewModel?> GetEditViewModelAsync(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return null;

            return new ClassViewModel
            {
                Id = @class.Id,
                Name = @class.Name,
                Price = @class.Price,
                StudentQuantity = @class.StudentQuantity,
                ClassLevel = @class.ClassLevel,
                Instructor = @class.InstructorId,
                City = @class.CityId
            };
        }

        public async Task<bool> UpdateClassAsync(int id, ClassViewModel model)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return false;

            @class.Update(model.Name, model.Price!.Value, model.StudentQuantity!.Value,
                          model.ClassLevel!.Value, model.Instructor!.Value, model.City!.Value);

            _context.Classes.Update(@class);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClassAsync(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return false;

            _context.Classes.Remove(@class);
            await _context.SaveChangesAsync();
            return true;
        }

        public SelectList GetInstructorsSelectList() =>
            new SelectList(_context.Instructors, "Id", "Name");

        public SelectList GetClassLevelSelectList() =>
            new SelectList(Enum.GetValues(typeof(ClassLevel)));

        public SelectList GetCitiesSelectList() =>
            new SelectList(_context.Cities, "Id", "Name");
    }
}
