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
                Name = signupDto.Name
            };
            var result = await _userManager.CreateAsync(user, signupDto.Password);
            if (!result.Succeeded)
                return null;

            // Only assign if role exists
            if (!await _roleManager.RoleExistsAsync(signupDto.Role))
            {
                // Role does not exist, return null or throw an exception as per your error handling strategy
                return null;
            }
            await _userManager.AddToRoleAsync(user, signupDto.Role);
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
    }
}