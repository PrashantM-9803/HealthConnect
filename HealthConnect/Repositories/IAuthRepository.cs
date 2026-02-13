using System.Collections.Generic;
using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace HealthConnect.Repositories
{
    public interface IAuthRepository
    {
        Task<(User user, IEnumerable<IdentityError> errors)> RegisterAsync(SignupRequestDto signupDto);
        Task<User> LoginAsync(LoginRequestDto loginDto);
        Task<User> GetByEmailAsync(string email);
        Task<(User user, bool valid)> RefreshTokenAsync(string email, string refreshToken);
    }
}