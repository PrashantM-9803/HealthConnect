using System;
using System.Threading.Tasks;
using AutoMapper;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<HealthConnect.Models.User> _userManager;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IImageRepository _imageRepository;

        public PatientController(IPatientRepository patientRepository, IMapper mapper, UserManager<HealthConnect.Models.User> userManager, IAppointmentRepository appointmentRepository, IImageRepository imageRepository)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;               
            _userManager = userManager;
            _appointmentRepository = appointmentRepository;
            _imageRepository = imageRepository;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "PATIENT,DOCTOR,ADMIN")]
        public async Task<IActionResult> GetPatientById(Guid id, [FromQuery] bool isUserId = false)
        {
            if (isUserId)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    return NotFound(new { message = "User not found." });
                var patient = await _patientRepository.GetPatientByUserIdAsync(id);
                if (patient == null)
                    return NotFound(new { message = "User exists but is not a patient." });
                var patientDto = _mapper.Map<PatientDto>(patient);
                return Ok(patientDto);
            }
            else
            {
                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient == null)
                    return NotFound(new { message = "Patient not found." });
                var patientDto = _mapper.Map<PatientDto>(patient);
                return Ok(patientDto);
            }
        }

        [HttpPut("update-profile/{userId}")]
        [Authorize(Roles = "PATIENT,DOCTOR,ADMIN")]
        public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] PatientUpdateProfileDto updateDto)
        {
            var result = await _patientRepository.UpdatePatientProfileAsync(userId, updateDto);
            if (!result)
                return NotFound(new { message = "Patient not found." });

            var updatedPatient = await _patientRepository.GetPatientByUserIdAsync(userId);
            if (updatedPatient == null)
                return NotFound(new { message = "Patient not found after update." });

            var patientDto = _mapper.Map<PatientDto>(updatedPatient);
            return Ok(patientDto);
        }

        // Create appointment
        [HttpPost("appointments")]
        [Authorize(Roles = "PATIENT")]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(dto.PatientId);
            if (patient == null)
                return BadRequest(new { message = "No patient found for this user." });

            dto.PatientId = patient.Id;
            var appointment = await _appointmentRepository.CreateAppointmentAsync(dto);
            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return Ok(appointmentDto);
        }

        // Get appointment by id
        [HttpGet("appointments/{appointmentId}")]
        [Authorize(Roles = "PATIENT,DOCTOR,ADMIN")]
        public async Task<IActionResult> GetAppointmentById(Guid appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound(new { message = "Appointment not found." });
            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return Ok(appointmentDto);
        }

        // Get all appointments for a patient
        [HttpGet("{UserId}/appointments")]
        [Authorize(Roles = "PATIENT,DOCTOR,ADMIN")]
        public async Task<IActionResult> GetAppointmentsByPatientId(Guid UserId)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(UserId);
            if (patient == null)
                return BadRequest(new { message = "No patient found for this user." });

            var PatientId = patient.Id;
            var appointments = await _appointmentRepository.GetAppointmentsByPatientIdAsync(PatientId);
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentDtos);
        }

        // Get all vitals for a patient
        [HttpGet("{patientId}/vitals")]
        [Authorize(Roles = "PATIENT,DOCTOR,ADMIN")]
        public async Task<IActionResult> GetVitalsByPatientId(Guid patientId)
        {

            var patient = await _patientRepository.GetPatientByIdAsync(patientId);
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            var vitals = patient.Vitals;
            if (vitals == null || vitals.Count == 0)
                return NotFound(new { message = "No vitals found for this patient." });

            var vitalsDto = _mapper.Map<IEnumerable<HealthConnect.Models.Dto.VitalsDto>>(vitals);
            return Ok(vitalsDto);
        }

        // Update profile image for a patient
        [HttpPost("update-profile-image/{userId}")]
        [Authorize(Roles = "PATIENT,ADMIN")]
        public async Task<IActionResult> UpdateProfileImage(Guid userId, [FromForm] ImageUploadDto imageUploadDto)
        {
            // Validate patient exists
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            // Validate image file
            if (!_imageRepository.ValidateImage(imageUploadDto.File))
                return BadRequest(new { message = "Invalid image file. Allowed formats: jpg, jpeg, png, gif, bmp. Max size: 5MB." });

            try
            {
                // Delete old profile image if exists
                if (!string.IsNullOrWhiteSpace(patient.ProfileImage))
                {
                    await _imageRepository.DeleteImageAsync(patient.ProfileImage);
                }

                // Upload new image
                var uploadedImage = await _imageRepository.UploadImageAsync(
                    imageUploadDto.File,
                    imageUploadDto.FileDescription,
                    "Patients"
                );

                // Update patient profile image path
                var updateResult = await _patientRepository.UpdatePatientProfileImageAsync(userId, uploadedImage.FilePath);
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

        // Delete profile image for a patient
        [HttpDelete("delete-profile-image/{userId}")]
        [Authorize(Roles = "PATIENT,ADMIN")]
        public async Task<IActionResult> DeleteProfileImage(Guid userId)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            if (string.IsNullOrWhiteSpace(patient.ProfileImage))
                return BadRequest(new { message = "No profile image to delete." });

            try
            {
                // Delete image file
                await _imageRepository.DeleteImageAsync(patient.ProfileImage);

                // Update patient profile image to null
                await _patientRepository.UpdatePatientProfileImageAsync(userId, null);

                return Ok(new { message = "Profile image deleted successfully." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the image." });
            }
        }
    }
}

