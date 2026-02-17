using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HealthConnect.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HealthConnect.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public ImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file size
            if (file.Length > MaxFileSize)
                return false;

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            return true;
        }

        public async Task<Image> UploadImageAsync(IFormFile file, string? description, string folder)
        {
            if (!ValidateImage(file))
                throw new ArgumentException("Invalid image file.");

            var image = new Image
            {
                Id = Guid.NewGuid(),
                File = file,
                FileName = file.FileName,
                FileDescription = description,
                FileExtension = Path.GetExtension(file.FileName).ToLowerInvariant(),
                FileSizeInBytes = file.Length
            };

            // Create unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{image.FileExtension}";

            // Ensure the folder exists in wwwroot
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Full file path
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file to disk
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Build full URL for the image
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var relativePath = $"/Images/{folder}/{uniqueFileName}";
            image.FilePath = $"{baseUrl}{relativePath}";

            return image;
        }

        public Task<bool> DeleteImageAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return Task.FromResult(false);

                // Extract relative path from full URL if it's a URL
                string relativePath = filePath;
                if (filePath.StartsWith("http://") || filePath.StartsWith("https://"))
                {
                    var uri = new Uri(filePath);
                    relativePath = uri.AbsolutePath.TrimStart('/');
                }

                // Build full physical path
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.Replace("/", "\\"));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
