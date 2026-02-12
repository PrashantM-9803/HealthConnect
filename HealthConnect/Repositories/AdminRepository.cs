using System;
using System.Threading.Tasks;
using HealthConnect.Models;
using Microsoft.AspNetCore.Identity;

namespace HealthConnect.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<User> _userManager;

        public AdminRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            // Generate password reset token and reset password (admin override)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }
    }
}
