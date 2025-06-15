using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using TFMGoSki.Data;
using TFMGoSki.Dtos;
using TFMGoSki.Exceptions;
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
                        RemainingStudentsQuantity = r.RemainingStudentsQuantity == -1 ? 0 : r.RemainingStudentsQuantity,
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

        public async Task<List<ClassDto>> GetAllClassesUserAsync(bool? finalizadas = null, string name = null, decimal? minPrice = null, decimal? maxPrice = null, string classLevel = null, string cityName = null, DateOnly? minDate = null, DateOnly? maxDate = null, int? minRating = null)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);
            var classDtos = new List<ClassDto>();

            // Obtener todas las clases (filtraremos después)
            var classes = await _context.Classes.ToListAsync();

            // Filtrar por nombre si se especifica
            if (!string.IsNullOrWhiteSpace(name))
            {
                classes = classes.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filtrar por precio mínimo
            if (minPrice.HasValue)
            {
                classes = classes.Where(c => c.Price >= minPrice.Value).ToList();
            }

            // Filtrar por precio máximo
            if (maxPrice.HasValue)
            {
                classes = classes.Where(c => c.Price <= maxPrice.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(classLevel))
            {
                classes = classes.Where(c => c.ClassLevel.ToFriendlyString().Equals(classLevel)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(cityName))
            {
                var cityIds = await _context.Cities
                    .Where(city => city.Name == cityName)
                    .Select(city => city.Id)
                    .ToListAsync();

                classes = classes.Where(c => cityIds.Contains(c.CityId)).ToList();
            }

            foreach (var clase in classes)
            {
                var cityFound = await _context.Cities.FindAsync(clase.CityId);
                var instructor = await _context.Instructors.FindAsync(clase.InstructorId);

                // Filtrar reservas por clase y por si están finalizadas o no
                var reservationsQuery = _context.ReservationTimeRangeClasses
                    .Where(r => r.ClassId == clase.Id);

                reservationsQuery = reservationsQuery.Where(r => r.StartDateOnly >= now || (r.StartDateOnly == now && r.StartTimeOnly >= nowTime));

                // Nuevo filtro por fecha mínima
                if (minDate.HasValue)
                {
                    reservationsQuery = reservationsQuery
                        .Where(r => r.StartDateOnly >= minDate.Value);
                }

                // Nuevo filtro por fecha máxima
                if (maxDate.HasValue)
                {
                    reservationsQuery = reservationsQuery
                        .Where(r => r.StartDateOnly <= maxDate.Value);
                }

                var reservationTimeRangeClassDtos = await reservationsQuery
                    .Select(r => new ReservationTimeRangeClassDto
                    {
                        RemainingStudentsQuantity = r.RemainingStudentsQuantity == -1 ? 0 : r.RemainingStudentsQuantity,
                        StartDateOnly = r.StartDateOnly,
                        EndDateOnly = r.EndDateOnly,
                        StartTimeOnly = r.StartTimeOnly,
                        EndTimeOnly = r.EndTimeOnly
                    })
                    .ToListAsync();

                if (!reservationTimeRangeClassDtos.Any()) continue; //interrumpe la iteración actual dentro de un bucle (for, while, do-while) y pasa a la siguiente iteración sin ejecutar el resto del código dentro del bucle

                var comments = await _context.ClassComments
                    .Where(cc => _context.ClassReservations
                        .Where(cr => cr.ClassId == clase.Id)
                        .Select(cr => cr.Id)
                        .Contains(cc.ClassReservationId))
                    .Join(_context.Users,
                        cc => _context.ClassReservations.FirstOrDefault(cr => cr.Id == cc.ClassReservationId).UserId,
                        u => u.Id,
                        (cc, u) => new ClassCommentDto
                        {
                            Text = cc.Text,
                            Raiting = cc.Raiting,
                            UserName = u.UserName
                        })
                    .ToListAsync();

                // Calcular el promedio de rating
                var averageRating = comments.Any() ? comments.Average(c => c.Raiting) : 0;

                // Filtrar por mínimo rating si está especificado
                if (minRating.HasValue && averageRating < minRating.Value)
                {
                    continue;
                }

                classDtos.Add(new ClassDto
                {
                    Id = clase.Id,
                    Name = clase.Name,
                    Price = clase.Price,
                    StudentQuantity = clase.StudentQuantity,
                    ClassLevel = clase.ClassLevel,
                    InstructorName = instructor?.Name,
                    CityName = cityFound?.Name,
                    ReservationTimeRangeClassDto = reservationTimeRangeClassDtos,
                    Comments = comments
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
                    Id = r.Id,
                    RemainingStudentsQuantity = r.RemainingStudentsQuantity == -1 ? 0 : r.RemainingStudentsQuantity,
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

        public async Task<UpdateResult> UpdateClassAsync(int id, ClassViewModel model)
        {
            var duplicate = _context.Classes
                .FirstOrDefault(c => c.Name.Equals(model.Name) && c.Id != id);

            if (duplicate != null)
            {
                return UpdateResult.Fail("A class with this name already exists.");
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return UpdateResult.Fail("Class not found.");
            }

            @class.Update(model.Name, model.Price!.Value, model.StudentQuantity!.Value,
                          model.ClassLevel!.Value, model.Instructor!.Value, model.City!.Value);

            _context.Classes.Update(@class);
            await _context.SaveChangesAsync();
            return UpdateResult.Ok();
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
