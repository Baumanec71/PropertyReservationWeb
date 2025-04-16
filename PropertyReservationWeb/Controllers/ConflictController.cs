using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyReservationWeb.Domain.ViewModels.Conflict;
using PropertyReservationWeb.Service.Interfaces;
using System.Security.Claims;

namespace PropertyReservationWeb.Controllers
{
    [Route("api/[controller]")]
    public class ConflictController : ControllerBase
    {
        private readonly IConflictService _conflictService;

        public ConflictController(IConflictService conflictService)
        {
            _conflictService = conflictService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetConflicts")]
        public async Task<IActionResult> GetConflicts(int page)
        {
            var filterModel = new ConflictFilterModel();
            var response = await _conflictService.GetConflicts(page, filterModel);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(response.Data);
            }

            return BadRequest(response.Description);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RejectedConflict")]
        public async Task<IActionResult> RejectedConflict([FromQuery] long id)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _conflictService.RejectedConflict(id, idUser);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(response.Data);
            }

            return BadRequest(response.Description);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ResolvedConflict")]
        public async Task<IActionResult> ResolvedConflict([FromQuery] long id)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _conflictService.ResolvedConflict(id, idUser);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(response.Data);
            }

            return BadRequest(response.Description);

        }
    }
}
