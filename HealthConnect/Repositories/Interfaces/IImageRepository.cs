using System.Threading.Tasks;
using HealthConnect.Models;
using Microsoft.AspNetCore.Http;

namespace HealthConnect.Repositories
{
    public interface IImageRepository
    {
        Task<Image> UploadImageAsync(IFormFile file, string? description, string folder);
        Task<bool> DeleteImageAsync(string filePath);
        bool ValidateImage(IFormFile file);
    }
}
