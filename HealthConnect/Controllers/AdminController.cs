using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using HealthConnect.Models.Dto;

namespace HealthConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;

        public AdminController(IPatientRepository patientRepository, IDoctorRepository doctorRepository, IAdminRepository adminRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        // GET: api/admin/patients
        [HttpGet("patients")]
        [Authorize(Roles ="ADMIN")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientRepository.GetAllPatientsAsync();
            var patientDtos = _mapper.Map<List<HealthConnect.Models.Dto.PatientDto>>(patients);
            return Ok(patientDtos);
        }

        // GET: api/admin/doctors
        [HttpGet("doctors")]
        [Authorize(Roles = "PATIENT,ADMIN")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _doctorRepository.GetAllDoctorsAsync();
            var doctorDtos = _mapper.Map<List<HealthConnect.Models.Dto.DoctorDto>>(doctors);
            return Ok(doctorDtos);
        }

        // GET: api/admin/appointments
        [HttpGet("appointments")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _adminRepository.GetAllAppointmentsAsync();
            var appointmentDtos = _mapper.Map<List<HealthConnect.Models.Dto.AppointmentDto>>(appointments);
            return Ok(appointmentDtos);
        }

        // GET: api/admin/appointments/pending
        [HttpGet("appointments/pending")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetPendingAppointments()
        {
            var appointments = await _adminRepository.GetPendingAppointmentsAsync();
            var appointmentDtos = _mapper.Map<List<HealthConnect.Models.Dto.AppointmentDto>>(appointments);
            return Ok(appointmentDtos);
        }

        // GET: api/admin/patients/total
        [HttpGet("patients/total")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTotalPatients()
        {
            var totalPatients = await _adminRepository.GetTotalPatientsAsync();
            return Ok(new TotalPatientsDto { TotalPatients = totalPatients });
        }

        // GET: api/admin/doctors/total
        [HttpGet("doctors/total")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTotalDoctors()
        {
            var totalDoctors = await _adminRepository.GetTotalDoctorsAsync();
            return Ok(new TotalDoctorsDto { TotalDoctors = totalDoctors });
        }

        // GET: api/admin/appointments/total
        [HttpGet("appointments/total")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTotalAppointments()
        {
            var (totalAppointments, todaysAppointments) = await _adminRepository.GetTotalAppointmentsAsync();
            return Ok(new 
            { 
                totalAppointments = totalAppointments,
                todaysAppointments = todaysAppointments
            });
        }

        // GET: api/admin/invoices/pending
        [HttpGet("invoices/pending")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetPendingInvoices()
        {
            var invoices = await _adminRepository.GetPendingInvoicesAsync();
            var invoiceDtos = _mapper.Map<List<HealthConnect.Models.Dto.InvoiceDto>>(invoices);
            return Ok(new
            {
                totalCount = invoiceDtos.Count,
                invoices = invoiceDtos
            });
        }

        // GET: api/admin/invoices/paid/total-amount
        [HttpGet("invoices/paid/total-amount")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTotalPaidInvoicesAmount()
        {
            var (totalAmount, todaysAmount) = await _adminRepository.GetTotalPaidInvoicesAmountAsync();
            return Ok(new
            {
                totalAmount = totalAmount,
                todaysAmount = todaysAmount
            });
        }

        // GET: api/admin/invoices
        [HttpGet("invoices")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _adminRepository.GetAllInvoicesAsync();
            var invoiceDtos = _mapper.Map<List<HealthConnect.Models.Dto.InvoiceDto>>(invoices);
            return Ok(new
            {
                totalCount = invoiceDtos.Count,
                invoices = invoiceDtos
            });
        }

        // DELETE: api/admin/patients/{userId}
        [HttpDelete("patients/{userId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeletePatient(Guid userId)
        {
            var result = await _adminRepository.DeletePatientAsync(userId);
            if (!result)
                return NotFound(new { message = "Patient not found." });
            return Ok(new { message = "Patient deleted successfully." });
        }

        // DELETE: api/admin/doctors/{userId}
        [HttpDelete("doctors/{userId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteDoctor(Guid userId)
        {
            var result = await _adminRepository.DeleteDoctorAsync(userId);
            if (!result)
                return NotFound(new { message = "Doctor not found." });
            return Ok(new { message = "Doctor deleted successfully." });
        }

        // PUT: api/admin/users/password/{userId}
        [HttpPut("users/password/{userId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UpdatePasswordDto dto, Guid userId)
        {
            var result = await _adminRepository.UpdateUserPasswordAsync(userId, dto.NewPassword);
            if (!result)
                return NotFound(new { message = "User not found or password update failed." });

            return Ok(new { message = "Password updated successfully." });
        }

    }
}
