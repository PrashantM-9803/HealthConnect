using System;
using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;

namespace HealthConnect.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientByIdAsync(Guid id);
        Task<Patient?> GetPatientByUserIdAsync(Guid userId);
        Task<bool> UpdatePatientProfileAsync(Guid userId, PatientUpdateProfileDto updateDto);
        Task<bool> UpdatePatientProfileImageAsync(Guid userId, string profileImagePath);
    }
}
