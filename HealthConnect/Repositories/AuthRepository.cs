using System.Threading.Tasks;
using HealthConnect.Data;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthConnect.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly HealthConnectDbContext _context;

        public AuthRepository(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, HealthConnectDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<User> RegisterAsync(SignupRequestDto signupDto)
        {
            var user = new User
            {
                UserName = signupDto.Email,
                Email = signupDto.Email,
                Name = signupDto.Name,
                PhoneNumber = signupDto.PhoneNumber,
                Dob = signupDto.Dob
            };

            // Only assign if role exists
            if (!await _roleManager.RoleExistsAsync(signupDto.Role))
                return null;

            var result = await _userManager.CreateAsync(user, signupDto.Password);
            if (!result.Succeeded)
                return null;

            await _userManager.AddToRoleAsync(user, signupDto.Role);

            // Automatically create Patient record if role is PATIENT
            if (signupDto.Role == "PATIENT")
            {
                var patient = new Patient
                {
                    UserId = user.Id,
                    MedicalHistory = string.Empty,
                    BloodGroup = string.Empty,
                    DoctorId = null // Now nullable
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            // Automatically create Doctor record if role is DOCTOR
            if (signupDto.Role == "DOCTOR")
            {
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Specialization = string.Empty,
                    YearsOfExperience = 0,
                    Bio = string.Empty
                };
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<User> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
                return null;
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            return isPasswordValid ? user : null;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        
        public async Task<(User user, bool valid)> RefreshTokenAsync(string email, string refreshToken)
        {
            var user = await GetByEmailAsync(email);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return (null, false);
            // Generate new refresh token and update expiry
            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            return (user, true);
        }
    }
}