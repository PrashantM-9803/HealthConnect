using System;
using System.Threading.Tasks;
using HealthConnect.Data;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
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

        public async Task<bool> UpdateDoctorProfileAsync(Guid userId, DoctorUpdateProfileDto updateDto)
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null || doctor.User == null)
                return false;

            doctor.User.Name = updateDto.Name;
           
            doctor.YearsOfExperience = updateDto.YearsOfExperience;
            doctor.Bio = updateDto.Bio;
           
            doctor.User.PhoneNumber = updateDto.PhoneNumber;
            // doctor.BID = updateDto.BID; // Add this if BID exists in Doctor model
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
