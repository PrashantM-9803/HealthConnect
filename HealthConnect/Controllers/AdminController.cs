using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace HealthConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public AdminController(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
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

        // DELETE: api/admin/patients/{id}
        [HttpDelete("patients/{patientId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeletePatient(Guid patientId)
        {
            var result = await _patientRepository.DeletePatientAsync(patientId);
            if (!result)
                return NotFound(new { message = "Patient not found." });
            return NoContent();
        }

    }
}
