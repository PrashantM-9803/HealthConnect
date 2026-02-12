using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthConnect.Data;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace HealthConnect.Repositories
{
    public class DoctorSlotRepository : IDoctorSlotRepository
    {
        private readonly HealthConnectDbContext _context;

        public DoctorSlotRepository(HealthConnectDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorSlot>> GenerateSlotsForDoctorAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            // Verify doctor exists
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
                return null;

            var slots = new List<DoctorSlot>();

            // Define working hours: 9 AM to 7 PM with 1-hour lunch break from 1 PM to 2 PM
            var morningSlots = new List<(TimeSpan start, TimeSpan end)>
            {
                (new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0)),   // 9 AM - 10 AM
                (new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0)),  // 10 AM - 11 AM
                (new TimeSpan(11, 0, 0), new TimeSpan(12, 0, 0)),  // 11 AM - 12 PM
                (new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0))   // 12 PM - 1 PM
            };

            var afternoonSlots = new List<(TimeSpan start, TimeSpan end)>
            {
                (new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0)),  // 2 PM - 3 PM
                (new TimeSpan(15, 0, 0), new TimeSpan(16, 0, 0)),  // 3 PM - 4 PM
                (new TimeSpan(16, 0, 0), new TimeSpan(17, 0, 0)),  // 4 PM - 5 PM
                (new TimeSpan(17, 0, 0), new TimeSpan(18, 0, 0)),  // 5 PM - 6 PM
                (new TimeSpan(18, 0, 0), new TimeSpan(19, 0, 0))   // 6 PM - 7 PM
            };

            var allTimeSlots = morningSlots.Concat(afternoonSlots).ToList();

            // Generate slots for each day in the date range
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                // Skip weekends (optional - remove if doctors work on weekends)
                // if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                //     continue;

                foreach (var timeSlot in allTimeSlots)
                {
                    // Check if slot already exists for this doctor, date, and time
                    var existingSlot = await _context.DoctorSlots
                        .FirstOrDefaultAsync(s => s.DoctorId == doctorId &&
                                                 s.Date.Date == date &&
                                                 s.StartTime == timeSlot.start &&
                                                 s.EndTime == timeSlot.end);

                    if (existingSlot == null)
                    {
                        var slot = new DoctorSlot
                        {
                            Id = Guid.NewGuid(),
                            DoctorId = doctorId,
                            Date = date,
                            StartTime = timeSlot.start,
                            EndTime = timeSlot.end,
                            IsBooked = false,
                            CreatedAt = DateTime.UtcNow
                        };

                        slots.Add(slot);
                        _context.DoctorSlots.Add(slot);
                    }
                }
            }

            if (slots.Any())
            {
                await _context.SaveChangesAsync();
            }

            return slots;
        }

        public async Task<DoctorSlot?> GetSlotByIdAsync(Guid slotId)
        {
            return await _context.DoctorSlots
                .Include(s => s.Doctor)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(s => s.Id == slotId);
        }

        public async Task<List<DoctorSlot>> GetAvailableSlotsByDoctorAsync(Guid doctorId, DateTime? date = null)
        {
            var query = _context.DoctorSlots
                .Where(s => s.DoctorId == doctorId && !s.IsBooked);

            if (date.HasValue)
            {
                query = query.Where(s => s.Date.Date == date.Value.Date);
            }
            else
            {
                // Only show future slots
                query = query.Where(s => s.Date >= DateTime.Today);
            }

            return await query
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<DoctorSlot>> GetAllSlotsByDoctorAsync(Guid doctorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.DoctorSlots
                .Where(s => s.DoctorId == doctorId);

            if (startDate.HasValue)
            {
                query = query.Where(s => s.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(s => s.Date <= endDate.Value.Date);
            }

            return await query
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<bool> MarkSlotAsBookedAsync(Guid slotId)
        {
            var slot = await _context.DoctorSlots.FindAsync(slotId);
            if (slot == null || slot.IsBooked)
                return false;

            slot.IsBooked = true;
            slot.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkSlotAsAvailableAsync(Guid slotId)
        {
            var slot = await _context.DoctorSlots.FindAsync(slotId);
            if (slot == null)
                return false;

            slot.IsBooked = false;
            slot.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSlotAsync(Guid slotId)
        {
            var slot = await _context.DoctorSlots.FindAsync(slotId);
            if (slot == null || slot.IsBooked)
                return false;

            _context.DoctorSlots.Remove(slot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSlotsForDateRangeAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            var slots = await _context.DoctorSlots
                .Where(s => s.DoctorId == doctorId &&
                           s.Date >= startDate.Date &&
                           s.Date <= endDate.Date &&
                           !s.IsBooked)
                .ToListAsync();

            if (!slots.Any())
                return false;

            _context.DoctorSlots.RemoveRange(slots);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
