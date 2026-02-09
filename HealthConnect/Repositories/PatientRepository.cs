using System;
using System.Threading.Tasks;
using HealthConnect.Data;
using HealthConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthConnect.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly HealthConnectDbContext _context;
        private readonly UserManager<User> _userManager;
        public PatientRepository(HealthConnectDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Patient?> GetPatientByIdAsync(Guid id)
        {
            return await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Doctor)
                .Include(p => p.Appointments)
                .Include(p => p.Vitals)
                .Include(p => p.Medications)
                .Include(p => p.Invoices)
                .Include(p => p.Diagnoses)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Patient?> GetPatientByUserIdAsync(Guid userId)
        {
            // Validate user exists using UserManager
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;
            return await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Doctor)
                .Include(p => p.Appointments)
                .Include(p => p.Vitals)
                .Include(p => p.Medications)
                .Include(p => p.Invoices)
                .Include(p => p.Diagnoses)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
