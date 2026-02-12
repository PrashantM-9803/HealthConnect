using System;
using System.Threading.Tasks;
using HealthConnect.Models;

namespace HealthConnect.Repositories
{
    public interface IAdminRepository
    {
        Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword);
        Task<bool> DeletePatientAsync(Guid patientId);
        Task<bool> DeleteDoctorAsync(Guid userId);
    }
}
