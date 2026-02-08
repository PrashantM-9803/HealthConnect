using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace HealthConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<User> _userManager;

        public AuthController(IAuthRepository authRepository, UserManager<User> userManager)
        {
            _authRepository = authRepository;
            _userManager = userManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDto signupDto)
        {
            var user = await _authRepository.RegisterAsync(signupDto);
            if (user == null)
                return BadRequest("User registration failed.");
            return Ok(new { user.Id, user.Email, user.Name }); // user.Id is now Guid
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var user = await _authRepository.LoginAsync(loginDto);
            if (user == null)
                return Unauthorized("Invalid credentials.");
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new LoginResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = roles.FirstOrDefault(),
                Token = null, // JWT to be added later
                RefreshToken = null,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
        }
    }
}