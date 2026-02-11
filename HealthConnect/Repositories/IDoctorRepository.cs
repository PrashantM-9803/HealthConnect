using System;
using System.Threading.Tasks;
using HealthConnect.Models;

namespace HealthConnect.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetDoctorByUserIdAsync(Guid userId);
        Task<Doctor?> GetDoctorByIdAsync(Guid id);
        Task<bool> UpdateDoctorProfileImageAsync(Guid userId, string profileImagePath);
    }
}
