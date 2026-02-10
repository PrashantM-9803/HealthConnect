using System;
using System.Linq;
using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace HealthConnect.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool Success, string ErrorMessage)> UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return (false, "User not found.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            return (true, string.Empty);
        }
    }
}
