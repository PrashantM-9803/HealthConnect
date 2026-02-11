using System;
using System.Threading.Tasks;
using HealthConnect.Data;
using HealthConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthConnect.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HealthConnectDbContext _context;
        public DoctorRepository(HealthConnectDbContext context)
        {
            _context = context;
        }

        public async Task<Doctor?> GetDoctorByUserIdAsync(Guid userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Patients)
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<Doctor?> GetDoctorByIdAsync(Guid id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Patients)
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> UpdateDoctorProfileImageAsync(Guid userId, string profileImagePath)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                return false;

            doctor.ProfileImage = profileImagePath;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
