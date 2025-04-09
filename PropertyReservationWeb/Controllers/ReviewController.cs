using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.ViewModels.RentalRequest;
using PropertyReservationWeb.Domain.ViewModels.Review;
using PropertyReservationWeb.Service.Implementations;
using PropertyReservationWeb.Service.Interfaces;
using System.Security.Claims;

namespace PropertyReservationWeb.Controllers
{
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpPut("DeleteReviewForUser")]
        public async Task<IActionResult> DeleteReviewForUser(long id)
        {
            var delreview = await _reviewService.DeleteReview(id);

            if (delreview.StatusCode == Domain.Enum.StatusCode.OK) return Ok(delreview.Description);

            return BadRequest(delreview.Description);
        }

        [Authorize]
        [HttpPost("CreateReview")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewViewModel model)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var newadvertisement = await _reviewService.CreateReview(model, id);

            if (newadvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(newadvertisement.Description);

            return BadRequest(newadvertisement.Description);
        }

        [Authorize]
        [HttpGet("GetReviewsForAdvertisement")]
        public async Task<IActionResult> GetReviewsForAdvertisement([FromQuery] long idAdvertisement, [FromQuery] int page = 1)
        {
            var defaultFilterModel = new ReviewFilterModel
            {
                SelectedIdAdvertisement = idAdvertisement,
                SelectedDeleteStatus = false,
            };

            var reviews = await _reviewService.GetReviews(page, defaultFilterModel);

            if (reviews.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(reviews.Data);
            }

            return BadRequest(reviews.Description);
        }

        [Authorize]
        [HttpGet("GetMyReviews")]
        public async Task<IActionResult> GetMyReviews([FromQuery] int page = 1)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var defaultFilterModel = new ReviewFilterModel
            {
                SelectedIdUserWhoReview = id,
                SelectedDeleteStatus = false,
            };

            var reviews = await _reviewService.GetReviews(page, defaultFilterModel);

            if (reviews.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(reviews.Data);
            }

            return BadRequest(reviews.Description);
        }

        [Authorize]
        [HttpGet("GetMySentReviews")]
        public async Task<IActionResult> GetMySentReviews([FromQuery] int page = 1)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var defaultFilterModel = new ReviewFilterModel
            {
                SelectedIdUserWhoReview = id,
                SelectedDeleteStatus = false,
            };

            var reviews = await _reviewService.GetReviews(page, defaultFilterModel);

            if (reviews.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(reviews.Data);
            }

            return BadRequest(reviews.Description);
        }

        [HttpPost("GetRentalRequestsFiltered")]
        public async Task<IActionResult> GetRentalRequestsFiltered([FromBody] ReviewFilterModel filterModel, [FromQuery] int page = 1)
        {
            var reviews = await _reviewService.GetReviews(page, filterModel);

            if (reviews.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(reviews.Data);
            }

            return BadRequest(reviews.Description);
        }
    }
}
