using System;
using System.Threading.Tasks;
using HealthConnect.Models.Dto;

namespace HealthConnect.Repositories
{
    public interface IUserRepository
    {
        Task<(bool Success, string ErrorMessage)> UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto);
    }
}
