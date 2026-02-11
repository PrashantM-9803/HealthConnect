using System;

namespace HealthConnect.Models.Dto
{
    public class ImageUploadResponseDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string? FileDescription { get; set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
