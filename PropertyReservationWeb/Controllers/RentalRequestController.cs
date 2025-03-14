using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.DAL.Repositories;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.Controllers
{
    [Route("api/[controller]")]
    public class RentalRequestController : Controller
    {
        private readonly IBaseRepository<RentalRequest> _rentalRequestRepository;

        public RentalRequestController(IBaseRepository<RentalRequest> rentalRequestRepository) //, ILogger logger
        {
            _rentalRequestRepository = rentalRequestRepository;
        }



        [HttpGet("GetRentalRequests")]
        public async Task<IActionResult> GetRentalRequests()
        {
            try
            {
                var rentalRequests = await _rentalRequestRepository.GetAll().AsNoTracking().ToListAsync();
                return Ok(rentalRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpPost("CreateRentalRequestEntity")]
        public async Task<IActionResult> CreateRentalRequestEntity([FromBody] RentalRequest rentalRequest)
        {
            try
            {

                await _rentalRequestRepository.Create(rentalRequest);
                return Ok(rentalRequest);
               // return Ok(new { message = "RentalRequests created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetRentalRequestsEntity")]
        public async Task<IActionResult> GetRentalRequestsEntity([FromQuery] int page = 1, [FromQuery] int pageSize = 1000)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                {
                    return BadRequest("Page and pageSize must be greater than 0.");
                }

                var totalRecords = await _rentalRequestRepository.GetAll().AsNoTracking().CountAsync(); // Общее количество записей
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);  // Общее количество страниц

                var rentalRequests = await _rentalRequestRepository.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).Include(x => x.User).Include(y => y.Advertisement).Include(z => z.Review)
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalRecords = totalRecords,
                    totalPages = totalPages,
                    rentalRequests = rentalRequests
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
