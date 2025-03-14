using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.Controllers
{
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly IBaseRepository<Review> _reviewRepository;

        public ReviewController(IBaseRepository<Review> reviewRepository) //, ILogger logger
        {
            _reviewRepository = reviewRepository;
        }



        //[HttpGet("GetReviews")]
        //public async Task<IActionResult> GetReviews()
        //{
        //    try
        //    {
        //        var rentalRequests = await _reviewRepository.GetAllEntity().AsNoTracking().ToListAsync();
        //        return Ok(rentalRequests);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error: {ex.Message}");
        //    }
        //}


        //[HttpPost("CreateReview")]
        //public async Task<IActionResult> CreateReviewEntity([FromBody] Review review)
        //{
        //    try
        //    {

        //        await _reviewRepository.Create(review);
        //        return Ok(review);
        //       // return Ok(new { message = "Reviews created successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //}

     
    }
}
