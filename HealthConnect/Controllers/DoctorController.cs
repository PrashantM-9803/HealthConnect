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
        // Delete profile image for a doctor
    }
}
