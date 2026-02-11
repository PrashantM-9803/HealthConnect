using System.ComponentModel.DataAnnotations.Schema;

namespace HealthConnect.Models
{
    public class Image
    {
        public Guid Id { get; set; }

        // property or class should be excluded from database mapping (do not create a column for it)
        [NotMapped]
        public IFormFile File { get; set; }

        public string FileName { get; set; }

        public string? FileDescription { get; set; }

        public string FileExtension { get; set; }

        public long FileSizeInBytes { get; set; }

        public string FilePath { get; set; }
    
}
}
