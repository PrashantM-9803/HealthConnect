using System;
using System.Threading.Tasks;
using AutoMapper;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace HealthConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<HealthConnect.Models.User> _userManager;

        public PatientController(IPatientRepository patientRepository, IMapper mapper, UserManager<HealthConnect.Models.User> userManager)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
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
    }
}
