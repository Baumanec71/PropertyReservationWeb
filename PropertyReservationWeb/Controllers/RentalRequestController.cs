using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyReservationWeb.Domain.ViewModels.RentalRequest;
using PropertyReservationWeb.Service.Interfaces;
using System.Security.Claims;

namespace PropertyReservationWeb.Controllers
{
    [Route("api/[controller]")]
    public class RentalRequestController : Controller
    {

        private readonly IRentalRequestService _rentalRequestService;
        private readonly IAdvertisementService _advertisementService;
        public RentalRequestController(IRentalRequestService rentalRequestService, IAdvertisementService advertisementService)
        {
            _rentalRequestService = rentalRequestService;
            _advertisementService = advertisementService;
        }

        [Authorize]
        [HttpGet("GetRentalRequest")]
        public async Task<IActionResult> GetRentalRequest([FromQuery] long id)
        {
            var advertisement = await _rentalRequestService.GetRentalRequest(id);

            if (advertisement.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisement.Data);
            }

            return BadRequest(advertisement.Description);
        }

        [Authorize]
        [HttpPut("DeleteRentalRequestForUser")]
        public async Task<IActionResult> DeleteRentalRequestForUser(long id)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var deladvertisement = await _rentalRequestService.DeleteRentalRequestForUser(id, idUser);

            if (deladvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(deladvertisement.Description);

            return BadRequest(deladvertisement.Description);
        }

        [Authorize]
        [HttpGet("CreateRentalRequest")]
        public async Task<IActionResult> GetRentalRequestForm([FromQuery] long id)
        {
            var model = new CreateRentalRequestViewModel();
            var getAllBookedDates = await _rentalRequestService.GetAllBookedDates(id);
            var needAdvertisement = await _advertisementService.GetAdvertisement(id);
            if (getAllBookedDates.StatusCode == Domain.Enum.StatusCode.OK && needAdvertisement.StatusCode == Domain.Enum.StatusCode.OK)
            {
                model.BookedDates = getAllBookedDates.Data!;
                model.RentalPrice = (decimal)needAdvertisement.Data!.RentalPrice!;
            }

            return Ok(model);
        }

        [Authorize]
        [HttpPost("CreateRentalRequest")]
        public async Task<IActionResult> CreateRentalRequest([FromBody] CreateRentalRequestViewModel model)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var newadvertisement = await _rentalRequestService.CreateRentalRequest(model, id);

            if (newadvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(newadvertisement.Description);

            return BadRequest(newadvertisement.Description);
        }

        [Authorize]
        [HttpPut("CreateApprovalStatusTrueAdvertisementForUser")]
        public async Task<IActionResult> CreateApprovalStatusTrueAdvertisementForUser(long id, decimal fixedPrepaymentAmount, decimal fixedDepositAmount, bool isPhotoSkippedByLandlord)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rentalApproved = await _rentalRequestService.CreateApprovalStatusTrueAdvertisementForUser(id, idUser, fixedPrepaymentAmount, fixedDepositAmount, isPhotoSkippedByLandlord);

            if (rentalApproved.StatusCode == Domain.Enum.StatusCode.OK) return Ok(rentalApproved.Description);

            return BadRequest(rentalApproved.Description);
        }


        [Authorize]
        [HttpPut("CreateApprovalStatusFalseAdvertisementForUser")]
        public async Task<IActionResult> CreateApprovalStatusFalseAdvertisementForUser(long id)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rentalfalseApproved = await _rentalRequestService.CreateApprovalStatusFalseAdvertisementForUser(id, idUser);

            if (rentalfalseApproved.StatusCode == Domain.Enum.StatusCode.OK) return Ok(rentalfalseApproved.Description);

            return BadRequest(rentalfalseApproved.Description);
        }

        [Authorize]
        [HttpPut("CreateApprovalStatusTheLandlordIsUnhappyAdvertisementForUser")]
        public async Task<IActionResult> CreateApprovalStatusTheLandlordIsUnhappyAdvertisementForUser(long id, string description)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rentalfalseApproved = await _rentalRequestService.CreateApprovalStatusTheLandlordIsUnhappyAdvertisementForUser(id, idUser, description);

            if (rentalfalseApproved.StatusCode == Domain.Enum.StatusCode.OK) return Ok(rentalfalseApproved.Description);

            return BadRequest(rentalfalseApproved.Description);
        }

        [Authorize]
        [HttpPut("CreateApprovalStatusTheBookingHasStartedAdvertisementForUser")]
        public async Task<IActionResult> CreateApprovalStatusTheBookingHasStartedAdvertisementForUser(long id)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rentalfalseApproved = await _rentalRequestService.CreateApprovalStatusTheBookingHasStartedAdvertisementForUser(id, idUser);

            if (rentalfalseApproved.StatusCode == Domain.Enum.StatusCode.OK) return Ok(rentalfalseApproved.Description);

            return BadRequest(rentalfalseApproved.Description);
        }

        [Authorize]
        [HttpPut("CreateApprovalStatusTheTenantIsUnhappyAdvertisementForUser")]
        public async Task<IActionResult> CreateApprovalStatusTheTenantIsUnhappyAdvertisementForUser(long id, string description)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rentalfalseApproved = await _rentalRequestService.CreateApprovalStatusTheTenantIsUnhappyAdvertisementForUser(id, idUser, description);

            if (rentalfalseApproved.StatusCode == Domain.Enum.StatusCode.OK) return Ok(rentalfalseApproved.Description);

            return BadRequest(rentalfalseApproved.Description);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllRentalRequests")]
        public async Task<IActionResult> GetAllRentalRequests([FromQuery] long idAdvertisement, [FromQuery] int page = 1)
        {
            var defaultFilterModel = new RentalRequestFilterModel
            {
                types = await _rentalRequestService.GetAllApprovalStatus(),
                SelectedIdNeedAdvertisement = idAdvertisement,
            };

            var rentalRequests = await _rentalRequestService.GetRentalRequests(page, defaultFilterModel);

            if (rentalRequests.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(rentalRequests.Data);
            }

            return BadRequest(rentalRequests.Description);
        }

        [HttpPost("GetAllRentalRequestsFiltered")]
        public async Task<IActionResult> GetAllRentalRequestsFiltered([FromBody] RentalRequestFilterModel filterModel, [FromQuery] int page = 1)
        {
            if (filterModel.types.Count == 0)
            {
                filterModel.types = await _rentalRequestService.GetAllApprovalStatus();
            }

            var rentalRequests = await _rentalRequestService.GetRentalRequests(page, filterModel);

            if (rentalRequests.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(rentalRequests.Data);
            }

            return BadRequest(rentalRequests.Description);
        }

        [Authorize]
        [HttpGet("GetRentalRequests")]
        public async Task<IActionResult> GetRentalRequests([FromQuery] long idAdvertisement, [FromQuery] int page = 1)
        {
            var defaultFilterModel = new RentalRequestFilterModel
            {
                types = await _rentalRequestService.GetAllApprovalStatus(),
                SelectedIdNeedAdvertisement = idAdvertisement,
                SelectedDeleteStatus = false,
            };

            var rentalRequests = await _rentalRequestService.GetRentalRequests(page, defaultFilterModel);

            if (rentalRequests.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(rentalRequests.Data);
            }

            return BadRequest(rentalRequests.Description);
        }

        [HttpPost("GetRentalRequestsFiltered")]
        public async Task<IActionResult> GetRentalRequestsFiltered([FromBody] RentalRequestFilterModel filterModel, [FromQuery] int page = 1)
        {
            if (filterModel.types.Count == 0)
            {
                filterModel.types = await _rentalRequestService.GetAllApprovalStatus();
            }

            filterModel.SelectedDeleteStatus = false;

            var rentalRequests = await _rentalRequestService.GetRentalRequests(page, filterModel);

            if (rentalRequests.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(rentalRequests.Data);
            }

            return BadRequest(rentalRequests.Description);
        }

        [Authorize]
        [HttpGet("GetMyRentalRequests")]
        public async Task<IActionResult> GetMyRentalRequests([FromQuery] int page = 1)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var advertisements = await _rentalRequestService.GetRentalRequests(page, new RentalRequestFilterModel() { SelectedIdAuthorNeedAdvertisement = id, SelectedDeleteStatus = false});

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK) return Ok(advertisements.Data);

            return BadRequest(advertisements.Description);
        }

        [Authorize]
        [HttpGet("GetMySentRentalRequests")]
        public async Task<IActionResult> GetMySentRentalRequests([FromQuery] int page = 1)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var advertisements = await _rentalRequestService.GetRentalRequests(page, new RentalRequestFilterModel() { SelectedIdAuthorRentalRequest = id, SelectedDeleteStatus = false });

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK) return Ok(advertisements.Data);

            return BadRequest(advertisements.Description);
        }
    }
}
