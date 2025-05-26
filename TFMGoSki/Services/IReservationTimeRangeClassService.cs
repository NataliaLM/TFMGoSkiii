using TFMGoSki.Dtos;
using TFMGoSki.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using TFMGoSki.Exceptions;

namespace TFMGoSki.Services
{
    public interface IReservationTimeRangeClassService
    {
        Task<List<ReservationTimeRangeClassDto>> GetAllAsync();
        Task<ReservationTimeRangeClassDto?> GetByIdAsync(int id);
        Task<ReservationTimeRangeClassViewModel?> GetEditViewModelAsync(int id);
        Task<UpdateReservationResult> CreateAsync(ReservationTimeRangeClassViewModel model);
        Task<UpdateReservationResult> UpdateAsync(int id, ReservationTimeRangeClassViewModel model);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);

        #region selectlist
        Task<SelectList> GetClassSelectListAsync();
        #endregion
    }
}
