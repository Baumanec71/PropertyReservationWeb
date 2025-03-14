using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyReservationWeb.Domain.ViewModels.Account;
using PropertyReservationWeb.Service.Interfaces;
using System.Security.Claims;

namespace PropertyReservationWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        public AuthController(IAccountService accountService, IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var response = await _accountService.Login(model);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                HttpContext.Response.Cookies.Append("my-cookies", response.Data!);
                return Ok(new { authenticated = true, token = response.Data });
            }

            return Unauthorized(new { error = "Неверные учетные данные" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            var response = await _accountService.Register(model);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
                return Ok(response.Description);

            return BadRequest(new { error = response.Description });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if(string.IsNullOrEmpty(email))
            {
                return BadRequest(new { error = "Вы не авторезированны" });
            }

            var response = await _accountService.ChangePassword(model, email);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
                return Ok(response.Description);

            return BadRequest(new { error = response.Description });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new
            {
                Message = "Выход из аккаунта завершен"
            });
        }

        [Authorize]
        [HttpPost("logoutForSwagger")]
        public async Task<IActionResult> LogoutForSwagger([FromQuery] string email)
        {
            var response = await _accountService.Logout(email);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
                return Ok(new { Message = "Выход из аккаунта завешен" });

            return BadRequest(response.Description);
        }
    }
}
