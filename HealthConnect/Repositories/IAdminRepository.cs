using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthConnect.Models;

namespace HealthConnect.Repositories
{
    public interface IAdminRepository
    {
        Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword);
        Task<bool> DeletePatientAsync(Guid userId);
        Task<bool> DeleteDoctorAsync(Guid userId);
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<int> GetTotalPatientsAsync();
        Task<int> GetTotalAppointmentsAsync();
    }
}
