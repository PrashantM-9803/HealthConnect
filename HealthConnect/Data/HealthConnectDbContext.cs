using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HealthConnect.Models;

namespace HealthConnect.Data
{
    public class HealthConnectDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public HealthConnectDbContext(DbContextOptions<HealthConnectDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Additional configuration if needed
        }
    }
}