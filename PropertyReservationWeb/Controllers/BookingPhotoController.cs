using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyReservationWeb.Domain.ViewModels.BookingPhoto;
using PropertyReservationWeb.Service.Interfaces;
using System.Security.Claims;

namespace PropertyReservationWeb.Controllers
{
    [Route("api/[controller]")]
    public class BookingPhotoController : ControllerBase
    {
        private readonly IBookingPhotoService _bookingPhotoService;

        public BookingPhotoController(IBookingPhotoService bookingPhotoService)
        {
            _bookingPhotoService = bookingPhotoService;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddPhotoAsync([FromBody] List<CreateBookingPhotoViewModel> photos, [FromQuery] long rentalRequestId)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _bookingPhotoService.AddPhotoAsync(photos, rentalRequestId, id);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
                return Ok(response);

            return BadRequest(response);
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetPhotosAsync([FromQuery] long rentalRequestId, [FromQuery] bool? isBefore = null)
        {
            var response = await _bookingPhotoService.GetPhotosAsync(rentalRequestId, isBefore);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
                return Ok(response);

            return BadRequest(response);
        }
    }
}