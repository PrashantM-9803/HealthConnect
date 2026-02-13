using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;

namespace HealthConnect.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientByIdAsync(Guid id);
        Task<Patient?> GetPatientByUserIdAsync(Guid userId);
        Task<List<Patient>> GetAllPatientsAsync();
        Task<bool> UpdatePatientProfileAsync(Guid userId, PatientUpdateProfileDto updateDto);
        Task<bool> UpdatePatientProfileImageAsync(Guid userId, string profileImagePath);
    }
}
