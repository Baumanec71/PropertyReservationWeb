using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.ViewModels.User;
using PropertyReservationWeb.Service.Interfaces;
using System.Security.Claims;

namespace PropertyReservationWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUserId")]
        public async Task<IActionResult> GetUserId([FromQuery] long id)
        {
            var user = await _userService.GetUserId(id);

            if(user.StatusCode == Domain.Enum.StatusCode.OK) return Ok(user);

            return BadRequest(new { error = user.Description });
            
        }

        [HttpGet("GetUserEmail")]
        public async Task<IActionResult> GetUserEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserEmail(email);

            if (user.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(user);
            }
            
            return BadRequest(new { error = user.Description });
        }


        [Authorize]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1)
        {
            var users = await _userService.GetUsers(page);

            if (users.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(users.Data);
            }

            return BadRequest(new { error = users.Description });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest(new { error = "Ошибка: Email пользователя не найден в токене." });
            }

            var user = await _userService.GetUserEmail(userEmail);

            if (user.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(user.Data);
            }

            return BadRequest(new { error = user.Description });
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserViewModel model)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.Update(model, id);

            if (user.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(new { description = user.Description });
            }

            return BadRequest(new { error = user.Description });
        }
    }
}