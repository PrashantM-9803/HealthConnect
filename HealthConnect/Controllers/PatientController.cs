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
        // Delete profile image for a patient
        
        
    }
}

