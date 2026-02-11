using System;
using System.Threading.Tasks;
using AutoMapper;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace HealthConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<HealthConnect.Models.User> _userManager;
        private readonly IImageRepository _imageRepository;

        public DoctorController(IDoctorRepository doctorRepository, IMapper mapper, UserManager<HealthConnect.Models.User> userManager, IImageRepository imageRepository)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
            _userManager = userManager;
            _imageRepository = imageRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(Guid id, [FromQuery] bool isUserId = false)
        {
            if (isUserId)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    return NotFound(new { message = "User not found." });
                var doctor = await _doctorRepository.GetDoctorByUserIdAsync(id);
                if (doctor == null)
                    return NotFound(new { message = "User exists but is not a doctor." });
                var doctorDto = _mapper.Map<DoctorDto>(doctor);
                return Ok(doctorDto);
            }
            else
            {
                var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
                if (doctor == null)
                    return NotFound(new { message = "Doctor not found." });
                var doctorDto = _mapper.Map<DoctorDto>(doctor);
                return Ok(doctorDto);
            }
        }

        // Update profile image for a doctor
        [HttpPost("update-profile-image/{userId}")]
        [Authorize(Roles = "DOCTOR,ADMIN")]
        public async Task<IActionResult> UpdateProfileImage(Guid userId, [FromForm] ImageUploadDto imageUploadDto)
        {
            // Validate doctor exists
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            // Validate image file
            if (!_imageRepository.ValidateImage(imageUploadDto.File))
                return BadRequest(new { message = "Invalid image file. Allowed formats: jpg, jpeg, png, gif, bmp. Max size: 5MB." });

            try
            {
                // Delete old profile image if exists
                if (!string.IsNullOrWhiteSpace(doctor.ProfileImage))
                {
                    await _imageRepository.DeleteImageAsync(doctor.ProfileImage);
                }

                // Upload new image
                var uploadedImage = await _imageRepository.UploadImageAsync(
                    imageUploadDto.File,
                    imageUploadDto.FileDescription,
                    "Doctors"
                );

                // Update doctor profile image path
                var updateResult = await _doctorRepository.UpdateDoctorProfileImageAsync(userId, uploadedImage.FilePath);
                if (!updateResult)
                    return StatusCode(500, new { message = "Failed to update profile image." });

                var response = new ImageUploadResponseDto
                {
                    FileName = uploadedImage.FileName,
                    FilePath = uploadedImage.FilePath,
                    FileDescription = uploadedImage.FileDescription,
                    FileExtension = uploadedImage.FileExtension,
                    FileSizeInBytes = uploadedImage.FileSizeInBytes,
                    UploadedAt = DateTime.UtcNow
                };

                return Ok(new
                {
                    message = "Profile image updated successfully.",
                    data = response
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while uploading the image." });
            }
        }

        // Delete profile image for a doctor
        [HttpDelete("delete-profile-image/{userId}")]
        [Authorize(Roles = "DOCTOR,ADMIN")]
        public async Task<IActionResult> DeleteProfileImage(Guid userId)
        {
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            if (string.IsNullOrWhiteSpace(doctor.ProfileImage))
                return BadRequest(new { message = "No profile image to delete." });

            try
            {
                // Delete image file
                await _imageRepository.DeleteImageAsync(doctor.ProfileImage);

                // Update doctor profile image to null
                await _doctorRepository.UpdateDoctorProfileImageAsync(userId, null);

                return Ok(new { message = "Profile image deleted successfully." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the image." });
            }
        }
    }
}
