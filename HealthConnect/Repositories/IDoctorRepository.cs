using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Generic;
using HealthConnect.Models;
using HealthConnect.Models.Dto;

namespace HealthConnect.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetDoctorByUserIdAsync(Guid userId);
        Task<Doctor?> GetDoctorByIdAsync(Guid id);
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<bool> UpdateDoctorProfileImageAsync(Guid userId, string profileImagePath);
        Task<bool> UpdateDoctorProfileAsync(Guid userId, DoctorUpdateProfileDto updateDto);
        Task<List<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId); // New method
        Task<Patient> GetPatientByIdAsync(Guid patientId);
    }
}
