using System;
using System.Threading.Tasks;
using HealthConnect.Models;

namespace HealthConnect.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientByIdAsync(Guid id);
        Task<Patient?> GetPatientByUserIdAsync(Guid userId);
    }
}
