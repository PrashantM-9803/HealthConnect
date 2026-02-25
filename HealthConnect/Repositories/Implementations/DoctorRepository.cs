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

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Patients)
                .Include(d => d.Appointments)
                .Include(d => d.DoctorSlots)
                .ToListAsync();
        }

        public async Task<Doctor?> GetDoctorByUserIdAsync(Guid userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Patients)
                .ThenInclude(p => p.User)
                .Include(d => d.Appointments)
                
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<Doctor?> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Appointments)
                .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor != null)
            {
                // Get all distinct patients who have appointments with this doctor
                var patientIds = doctor.Appointments.Select(a => a.PatientId).Distinct().ToList();
                var patients = await _context.Patients
                    .Where(p => patientIds.Contains(p.Id))
                    .Include(p => p.User)
                    .ToListAsync();
                doctor.Patients = patients;
            }
            return doctor;
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
            doctor.Specialization = updateDto.Specialization; // Set specialization
            doctor.YearsOfExperience = updateDto.YearsOfExperience;
            doctor.Bio = updateDto.Bio;
            doctor.User.PhoneNumber = updateDto.PhoneNumber;
            // doctor.BID = updateDto.BID; // Add this if BID exists in Doctor model
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId)
        {
            // Get all distinct patients who have booked appointments with the doctor
            var patientIds = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            return await _context.Patients
                .Where(p => patientIds.Contains(p.Id))
                .Include(p => p.User)
                .Include(p => p.Vitals)
                .Include(p => p.Medications)
                .Include(p => p.Invoices)
                .Include(p => p.Diagnoses)
                .Include(p => p.Appointments.Where(a => a.Status == AppointmentStatus.Completed))
                    .ThenInclude(a => a.Diagnosis)
                .Include(p => p.Appointments.Where(a => a.Status == AppointmentStatus.Completed))
                    .ThenInclude(a => a.Vitals)
                .Include(p => p.Appointments.Where(a => a.Status == AppointmentStatus.Completed))
                    .ThenInclude(a => a.Medications)
                .ToListAsync();
        }

        public async Task<Patient> GetPatientByIdAsync(Guid patientId)
        {
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }
    }
}
