using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace HealthConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<User> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly IMapper _mapper;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;

        public AuthController(
            IAuthRepository authRepository,
            UserManager<User> userManager,
            ITokenRepository tokenRepository,
            IMapper mapper,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository)
        {
            _authRepository = authRepository;
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDto signupDto)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(signupDto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User with this email already exists." });

            var (user, errors) = await _authRepository.RegisterAsync(signupDto);
            if (user == null)
            {
                var errorMessages = errors.Select(e => e.Description);
                return BadRequest(new { message = "User registration failed.", errors = errorMessages });
            }
            
            var userDto = _mapper.Map<LoginResponseDto>(user);
            userDto.Role = signupDto.Role;

            // Set PatientId/DoctorId if available
            if (signupDto.Role == "PATIENT")
            {
                var patient = await _patientRepository.GetPatientByUserIdAsync(user.Id);
                userDto.PatientId = patient?.Id;
            }
            else if (signupDto.Role == "DOCTOR")
            {
                var doctor = await _doctorRepository.GetDoctorByUserIdAsync(user.Id);
                userDto.DoctorId = doctor?.Id;
            }
            return Ok(userDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var user = await _authRepository.LoginAsync(loginDto);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(loginDto.Role))
                return Unauthorized("Role mismatch.");

            var token = _tokenRepository.GenerateJwtToken(user, roles);
            var refreshToken = _tokenRepository.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            var responseDto = _mapper.Map<LoginResponseDto>(user);

            responseDto.Role = roles.FirstOrDefault();
            responseDto.Token = token;
            responseDto.RefreshToken = refreshToken;
            responseDto.ExpiresAt = DateTime.UtcNow.AddHours(1);

            // Set PatientId/DoctorId if available
            var patient = await _patientRepository.GetPatientByUserIdAsync(user.Id);
            if (patient != null)
                responseDto.PatientId = patient.Id;
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(user.Id);
            if (doctor != null)
                responseDto.DoctorId = doctor.Id;
            return Ok(responseDto);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto refreshDto)
        {
            var (user, valid) = await _authRepository.RefreshTokenAsync(refreshDto.Email, refreshDto.RefreshToken);
            if (!valid)
                return Unauthorized("Invalid or expired refresh token.");
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenRepository.GenerateJwtToken(user, roles);
            var responseDto = _mapper.Map<LoginResponseDto>(user);
            responseDto.Role = roles.FirstOrDefault();
            responseDto.Token = token;
            responseDto.RefreshToken = user.RefreshToken;
            responseDto.ExpiresAt = DateTime.UtcNow.AddHours(1);

            // Set PatientId/DoctorId if available
            var patient = await _patientRepository.GetPatientByUserIdAsync(user.Id);
            if (patient != null)
                responseDto.PatientId = patient.Id;
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(user.Id);
            if (doctor != null)
                responseDto.DoctorId = doctor.Id;
            return Ok(responseDto);
        }

        [HttpGet("admin-only")]
        [Authorize(Roles = "ADMIN")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "ADMIN")]
        public IActionResult AdminOnly()
        {
            return Ok("This endpoint is only accessible by ADMIN role.");
        }
    }
}