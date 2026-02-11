using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;

namespace HealthConnect.Repositories
{
    public interface IDoctorSlotRepository
    {
        Task<List<DoctorSlot>> GenerateSlotsForDoctorAsync(Guid doctorId, DateTime startDate, DateTime endDate);
        Task<DoctorSlot?> GetSlotByIdAsync(Guid slotId);
        Task<List<DoctorSlot>> GetAvailableSlotsByDoctorAsync(Guid doctorId, DateTime? date = null);
        Task<List<DoctorSlot>> GetAllSlotsByDoctorAsync(Guid doctorId, DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> MarkSlotAsBookedAsync(Guid slotId);
        Task<bool> MarkSlotAsAvailableAsync(Guid slotId);
        Task<bool> DeleteSlotAsync(Guid slotId);
        Task<bool> DeleteSlotsForDateRangeAsync(Guid doctorId, DateTime startDate, DateTime endDate);
    }
}
