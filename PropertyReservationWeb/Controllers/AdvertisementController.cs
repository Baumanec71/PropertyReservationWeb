using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.OpenApi.Extensions;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Extensions;
using PropertyReservationWeb.Domain.ViewModels.Advertisement;
using PropertyReservationWeb.Domain.ViewModels.Amenity;
using PropertyReservationWeb.Service.Interfaces;
using System.Security.Claims;

namespace PropertyReservationWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisementController : Controller
    {
        private readonly IAdvertisementService _advertisementService;
        private readonly IAmenityService _amenityService;

        public AdvertisementController(IAdvertisementService advertisementService, IAmenityService amenityService)
        {
            _advertisementService = advertisementService;
            _amenityService = amenityService;
        }

        [Authorize]
        [HttpGet("GetAdvertisements")]
        public async Task<IActionResult> GetAdvertisements([FromQuery] int page = 1)
        {
            var defaultFilterModel = new AdvertisementFilterModel
            {
                types = await _advertisementService.GetAllObjectTypes(),
                CreateAdvertisementAmenities = (await _amenityService.GetAllAmenityTypes())
                    .Select(amenityType => new CreateAdvertisementAmenityViewModel
                    {
                        Amenity = amenityType,
                        AmenityDisplay = amenityType.GetDisplayName(),
                        IsActive = false,
                        Value = null
                    })
                    .ToList()
            };

            var advertisements = await _advertisementService.GetAdvertisements(page, defaultFilterModel);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisements.Data);
            }

            return BadRequest(advertisements.Description);
        }

        [Authorize]
        [HttpPost("GetAdvertisements")]
        public async Task<IActionResult> GetAdvertisementsFiltered([FromBody] AdvertisementFilterModel filterModel, [FromQuery] int page = 1)
        {
            if (filterModel.types.Count == 0)
            {
                filterModel.types = await _advertisementService.GetAllObjectTypes();
            }

            if (filterModel.CreateAdvertisementAmenities.Count == 0)
            {
                var amenityTypes = await _amenityService.GetAllAmenityTypes();

                if (amenityTypes.Count != 0)
                {
                    filterModel.CreateAdvertisementAmenities = amenityTypes
                        .Select(amenityType => new CreateAdvertisementAmenityViewModel
                        {
                            Amenity = amenityType,
                            AmenityDisplay = amenityType.GetDisplayName(),
                            IsActive = false,
                            Value = null
                        })
                        .ToList();
                }
            }

            var advertisements = await _advertisementService.GetAdvertisements(page, filterModel);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisements.Data);
            }

            return BadRequest(advertisements.Description);
        }

        //[HttpPost("GetAdvertisements")]
        //public async Task<IActionResult> GetAdvertisements([FromQuery] int page = 1, [FromBody] AdvertisementFilterModel? filterModel = null)
        //{

        //    if(filterModel!.types.Count == 0)
        //    {
        //        filterModel!.types = await _advertisementService.GetAllObjectTypes();
        //    }

        //    if(filterModel?.CreateAdvertisementAmenities.Count == 0)
        //    {
        //        var amenityTypes = await _amenityService.GetAllAmenityTypes();

        //        if (amenityTypes.Count != 0)
        //        {
        //            filterModel!.CreateAdvertisementAmenities = amenityTypes
        //                .Select(amenityType => new CreateAdvertisementAmenityViewModel
        //                {
        //                    Amenity = amenityType,
        //                    AmenityDisplay = amenityType.GetDisplayName(),
        //                    IsActive = false,
        //                    Value = null
        //                })
        //                .ToList();
        //        }
        //    }

        //    var advertisements = await _advertisementService.GetAdvertisements(page, filterModel ?? new AdvertisementFilterModel());

        //    if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
        //    {
        //        return Ok(advertisements.Data);
        //    }

        //    return BadRequest(advertisements.Description);
        //}

        [HttpGet("GetMyAdvertisements")]
        public async Task<IActionResult> GetMyAdvertisements([FromQuery] int page = 1)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var advertisements = await _advertisementService.GetMyAdvertisements(id, page);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisements.Data?.ToList());
            }

            return BadRequest(advertisements.Description);
        }

        [HttpGet("GetConfirmationAdvertisements")]
        public async Task<IActionResult> GetConfirmationAdvertisements([FromQuery] int page = 1)
        {
            var advertisements = await _advertisementService.GetConfirmationAdvertisements(page);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisements.Data?.ToList());
            }

            return BadRequest(advertisements.Description);
        }

        [HttpGet("GetMyNoDeleteAdvertisements")]
        public async Task<IActionResult> GetMyNoDeleteAdvertisements([FromQuery] int page = 1)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var advertisements = await _advertisementService.GetMyNoDeleteAdvertisements(id, page);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisements.Data?.ToList());
            }

            return BadRequest(advertisements.Description);
        }

        [HttpGet("GetAdvertisement/{id}")]
        public async Task<IActionResult> GetAdvertisement(int id)
        {
            var advertisement = await _advertisementService.GetAdvertisement(id);

            if (advertisement.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisement.Data);
            }

            return BadRequest(advertisement.Description);
        }

        [Authorize]
        [HttpGet("CreateAdvertisement")]
        public async Task<IActionResult> GetCreateAdvertisementForm()
        {
            var model = new CreateAdvertisementViewModel();


            model.types = await _advertisementService.GetAllObjectTypes();

            var amenityTypes = await _amenityService.GetAllAmenityTypes();

            if(amenityTypes.Count == 0)
            {
                return Ok(model);
            }

            model.CreateAdvertisementAmenities = amenityTypes
                .Select(amenityType => new CreateAdvertisementAmenityViewModel
                {
                    Amenity = amenityType,
                    AmenityDisplay = amenityType.GetDisplayName(),
                    IsActive = false,
                    Value = null
                })
                .ToList();

            return Ok(model);
        }

        [Authorize]
        [HttpPost("CreateAdvertisement")]
        public async Task<IActionResult> CreateAdvertisement([FromBody] CreateAdvertisementViewModel advertisement)
        {
            var newadvertisement = await _advertisementService.CreateAdvertisement(advertisement);

            if(newadvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(newadvertisement.Description);

            return BadRequest(newadvertisement.Description); 
        }

        [HttpPut("DeleteAdvertisementForUser")]
        public async Task<IActionResult> DeleteAdvertisementForUser(long id)
        {
            var deladvertisement = await _advertisementService.DeleteAdvertisementForUser(id);

            if (deladvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(deladvertisement.Description);

            return BadRequest(deladvertisement.Description);
        }

        [HttpPut("UpdateAdvertisement")]
        public async Task<IActionResult> UpdateAdvertisement([FromBody] AdvertisementViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deladvertisement = await _advertisementService.Edit(model);

            if (deladvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(deladvertisement.Description);           

            return BadRequest(deladvertisement.Description);
        }
    }
}
