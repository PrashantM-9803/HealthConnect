using System;
using System.Threading.Tasks;
using AutoMapper;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

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
        private readonly IAppointmentRepository _appointmentRepository;

        public DoctorController(
            IDoctorRepository doctorRepository, 
            IMapper mapper, 
            UserManager<HealthConnect.Models.User> userManager, 
            IImageRepository imageRepository,
            IAppointmentRepository appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
            _userManager = userManager;
            _imageRepository = imageRepository;
            _appointmentRepository = appointmentRepository;
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

        // Get all appointments for a doctor
        [HttpGet("appointments/{doctorId}")]
        [Authorize(Roles = "DOCTOR,ADMIN")]
        public async Task<IActionResult> GetAppointmentsByDoctorId(Guid doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            var appointments = await _appointmentRepository.GetAppointmentsByDoctorIdAsync(doctorId);
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            
            return Ok(new
            {
                doctorId = doctorId,
                totalAppointments = appointmentDtos.Count(),
                appointments = appointmentDtos
            });
        }

        // Update profile image for a doctor
        // Delete profile image for a doctor

        [HttpPut("update-profile/{userId}")]
        [Authorize(Roles = "DOCTOR,ADMIN")]
        public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] DoctorUpdateProfileDto updateDto)
        {
            var result = await _doctorRepository.UpdateDoctorProfileAsync(userId, updateDto);
            if (!result)
                return NotFound(new { message = "Doctor not found." });

            var updatedDoctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
            if (updatedDoctor == null)
                return NotFound(new { message = "Doctor not found after update." });

            var doctorDto = _mapper.Map<DoctorDto>(updatedDoctor);
            return Ok(doctorDto);
        }

        [HttpPost("appointments/diagnosis/{appointmentId}")]
        [Authorize(Roles = "DOCTOR,ADMIN")]
        public async Task<IActionResult> AddDiagnosis(Guid appointmentId, [FromBody] AddDiagnosisDto dto)
        {
            if (appointmentId != dto.AppointmentId)
                return BadRequest(new { message = "AppointmentId mismatch." });

            var diagnosis = await _appointmentRepository.AddDiagnosisAsync(dto);
            var diagnosisDto = _mapper.Map<DiagnosisDto>(diagnosis);
            return Ok(diagnosisDto);
        }
    }
}

