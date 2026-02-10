using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPut("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("id")?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var (success, error) = await _userRepository.UpdatePasswordAsync(userId, dto);
            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Password updated successfully." });
        }
    }
}
