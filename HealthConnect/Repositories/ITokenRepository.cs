using HealthConnect.Models;

namespace HealthConnect.Repositories
{
    public interface ITokenRepository
    {
        string GenerateJwtToken(User user, IList<string> roles);
        string GenerateRefreshToken();
    }
}
