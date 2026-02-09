using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;

namespace HealthConnect.Repositories
{
    public interface IAuthRepository
    {
        Task<User> RegisterAsync(SignupRequestDto signupDto);
        Task<User> LoginAsync(LoginRequestDto loginDto);
        Task<User> GetByEmailAsync(string email);
        Task<(User user, bool valid)> RefreshTokenAsync(string email, string refreshToken);
    }
}