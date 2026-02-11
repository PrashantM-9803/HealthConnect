using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HealthConnect.Models.Dto
{
    public class ImageUploadDto
    {
        [Required]
        public IFormFile File { get; set; }

        public string? FileDescription { get; set; }
    }
}
