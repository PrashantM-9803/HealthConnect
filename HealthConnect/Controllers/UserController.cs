using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IImageRepository _imageRepository;

        public UserController(
            IUserRepository userRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            IImageRepository imageRepository)
        {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _imageRepository = imageRepository;
        }

        [HttpPut("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("id")?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var (success, error) = await _userRepository.UpdatePasswordAsync(userId, dto);
            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Password updated successfully." });
        }

        /// <summary>
        /// Unified endpoint to delete profile image for both Patient and Doctor.
        /// Usage: /api/User/delete-profile-image/{userId}?userType=PATIENT or DOCTOR
        /// </summary>
        [HttpDelete("delete-profile-image/{userId}")]
        [Authorize(Roles = "PATIENT,DOCTOR,ADMIN")]
        public async Task<IActionResult> DeleteProfileImage(Guid userId, [FromQuery] string userType)
        {
            try
            {
                if (userType.Equals("PATIENT", StringComparison.OrdinalIgnoreCase))
                {
                    var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
                    if (patient == null)
                        return NotFound(new { message = "Patient not found." });

                    if (string.IsNullOrWhiteSpace(patient.ProfileImage))
                        return BadRequest(new { message = "No profile image to delete." });

                    await _imageRepository.DeleteImageAsync(patient.ProfileImage);
                    await _patientRepository.UpdatePatientProfileImageAsync(userId, null);

                    return Ok(new { message = "Profile image deleted successfully." });
                }
                else if (userType.Equals("DOCTOR", StringComparison.OrdinalIgnoreCase))
                {
                    var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
                    if (doctor == null)
                        return NotFound(new { message = "Doctor not found." });

                    if (string.IsNullOrWhiteSpace(doctor.ProfileImage))
                        return BadRequest(new { message = "No profile image to delete." });

                    await _imageRepository.DeleteImageAsync(doctor.ProfileImage);
                    await _doctorRepository.UpdateDoctorProfileImageAsync(userId, null);

                    return Ok(new { message = "Profile image deleted successfully." });
                }
                else
                {
                    return BadRequest(new { message = "Invalid userType. Must be 'PATIENT' or 'DOCTOR'." });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the image." });
            }
        }

        /// <summary>
        /// Unified endpoint to update profile image for both Patient and Doctor.
        /// Usage: /api/User/update-profile-image/{userId}?userType=PATIENT or DOCTOR
        /// </summary>
        [HttpPost("update-profile-image/{userId}")]
        [Authorize(Roles = "PATIENT,DOCTOR,ADMIN")]
        public async Task<IActionResult> UpdateProfileImage(Guid userId, [FromForm] ImageUploadDto imageUploadDto, [FromQuery] string userType)
        {
            if (!_imageRepository.ValidateImage(imageUploadDto.File))
                return BadRequest(new { message = "Invalid image file. Allowed formats: jpg, jpeg, png, gif, bmp. Max size: 5MB." });

            try
            {
                if (userType.Equals("PATIENT", StringComparison.OrdinalIgnoreCase))
                {
                    var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
                    if (patient == null)
                        return NotFound(new { message = "Patient not found." });

                    if (!string.IsNullOrWhiteSpace(patient.ProfileImage))
                        await _imageRepository.DeleteImageAsync(patient.ProfileImage);

                    var uploadedImage = await _imageRepository.UploadImageAsync(imageUploadDto.File, imageUploadDto.FileDescription, "Patients");
                    var updateResult = await _patientRepository.UpdatePatientProfileImageAsync(userId, uploadedImage.FilePath);
                    if (!updateResult)
                        return StatusCode(500, new { message = "Failed to update profile image." });

                    return Ok(new
                    {
                        message = "Profile image updated successfully.",
                        data = new ImageUploadResponseDto
                        {
                            FileName = uploadedImage.FileName,
                            FilePath = uploadedImage.FilePath,
                            FileDescription = uploadedImage.FileDescription,
                            FileExtension = uploadedImage.FileExtension,
                            FileSizeInBytes = uploadedImage.FileSizeInBytes,
                            UploadedAt = DateTime.UtcNow
                        }
                    });
                }
                else if (userType.Equals("DOCTOR", StringComparison.OrdinalIgnoreCase))
                {
                    var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
                    if (doctor == null)
                        return NotFound(new { message = "Doctor not found." });

                    if (!string.IsNullOrWhiteSpace(doctor.ProfileImage))
                        await _imageRepository.DeleteImageAsync(doctor.ProfileImage);

                    var uploadedImage = await _imageRepository.UploadImageAsync(imageUploadDto.File, imageUploadDto.FileDescription, "Doctors");
                    var updateResult = await _doctorRepository.UpdateDoctorProfileImageAsync(userId, uploadedImage.FilePath);
                    if (!updateResult)
                        return StatusCode(500, new { message = "Failed to update profile image." });

                    return Ok(new
                    {
                        message = "Profile image updated successfully.",
                        data = new ImageUploadResponseDto
                        {
                            FileName = uploadedImage.FileName,
                            FilePath = uploadedImage.FilePath,
                            FileDescription = uploadedImage.FileDescription,
                            FileExtension = uploadedImage.FileExtension,
                            FileSizeInBytes = uploadedImage.FileSizeInBytes,
                            UploadedAt = DateTime.UtcNow
                        }
                    });
                }
                else
                {
                    return BadRequest(new { message = "Invalid userType. Must be 'PATIENT' or 'DOCTOR'." });
                }
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
    }
}
