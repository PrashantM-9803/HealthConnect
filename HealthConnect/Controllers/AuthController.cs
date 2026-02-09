using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HealthConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<User> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(IAuthRepository authRepository, UserManager<User> userManager, ITokenRepository tokenRepository)
        {
            _authRepository = authRepository;
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        [HttpPost("signup")]
        //[Authorize(Roles="PATIENT, DOCTOR")]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDto signupDto)
        {
            var user = await _authRepository.RegisterAsync(signupDto);
            if (user == null)
                return BadRequest("User registration failed.");
            return Ok(new { user.Id, user.Email, user.Name, user.PhoneNumber }); // user.Id is Guid
        }


        [HttpPost("login")]
        //[Authorize(Roles = "PATIENT, DOCTOR, ADMIN")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var user = await _authRepository.LoginAsync(loginDto);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);

            // Check if provided role matches any of the user's roles
            if (!roles.Contains(loginDto.Role))
                return Unauthorized("Role mismatch.");

            var token = _tokenRepository.GenerateJwtToken(user, roles);
            var refreshToken = _tokenRepository.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            return Ok(new LoginResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = roles.FirstOrDefault(),
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
        }

        [HttpPost("refresh")]
        //[Authorize(Roles = "PATIENT, DOCTOR, ADMIN")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto refreshDto)
        {
            var (user, valid) = await _authRepository.RefreshTokenAsync(refreshDto.Email, refreshDto.RefreshToken);
            if (!valid)
                return Unauthorized("Invalid or expired refresh token.");
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenRepository.GenerateJwtToken(user, roles);
            return Ok(new LoginResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = roles.FirstOrDefault(),
                Token = token,
                RefreshToken = user.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
        }

        // Example of role-based access
        [HttpGet("admin-only")]
        [Authorize(Roles = "ADMIN")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "ADMIN")]
        public IActionResult AdminOnly()
        {
            return Ok("This endpoint is only accessible by ADMIN role.");
        }
    }
}