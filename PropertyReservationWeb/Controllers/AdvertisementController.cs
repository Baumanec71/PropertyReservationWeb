using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetAdvertisements")]
        public async Task<IActionResult> GetAdvertisements([FromQuery] int page = 1)
        {
            var defaultFilterModel = new AdvertisementFilterModel
            {
                types =  _advertisementService.GetAllObjectTypes(),
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

            var advertisements = await _advertisementService.GetAdvertisements<ShortAdvertisementViewModel>(page, defaultFilterModel, true, null, false);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisements.Data);
            }

            return BadRequest(advertisements.Description);
        }

        [HttpPost("GetAdvertisements")]
        public async Task<IActionResult> GetAdvertisementsFiltered([FromBody] AdvertisementFilterModel filterModel, [FromQuery] int page = 1)
        {
            if (filterModel.types.Count == 0)
            {
                filterModel.types = _advertisementService.GetAllObjectTypes();
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

            var advertisements = await _advertisementService.GetAdvertisements<ShortAdvertisementViewModel>(page, filterModel, true, null, false);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisements.Data);
            }

            return BadRequest(advertisements.Description);
        }


        [Authorize]
        [HttpGet("GetMyAdvertisements")]
        public async Task<IActionResult> GetMyAdvertisements( [FromQuery] int page = 1)
        {
            var id = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var advertisements = await _advertisementService.GetAdvertisements<ShortAdvertisementViewModel>(page, new AdvertisementFilterModel(), null, id, false);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK) return Ok(advertisements.Data);

            return BadRequest(advertisements.Description);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllAdvertisements")]
        public async Task<IActionResult> GetAllAdvertisements([FromQuery] int page = 1)
        {
            var defaultFilterModel = new AdvertisementFilterModel
            {
                types = _advertisementService.GetAllObjectTypes(),
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

            var advertisements = await _advertisementService.GetAdvertisements<AdvertisementViewModel>(page, defaultFilterModel, null, null, null);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisements.Data);
            }

            return BadRequest(advertisements.Description);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("GetAllAdvertisements")]
        public async Task<IActionResult> GetAllAdvertisementsFiltered([FromBody] AdvertisementFilterModel filterModel, [FromQuery] int page = 1)
        {
            if (filterModel.types.Count == 0)
            {
                filterModel.types = _advertisementService.GetAllObjectTypes();
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

            var advertisements = await _advertisementService.GetAdvertisements<AdvertisementViewModel>(page, filterModel, null, null, null);

            if (advertisements.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(advertisements.Data);
            }

            return BadRequest(advertisements.Description);
        }

        [Authorize]
        [HttpPut("DeleteAdvertisementForUser")]
        public async Task<IActionResult> DeleteAdvertisementForUser(long id)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var deladvertisement = await _advertisementService.DeleteAdvertisementForUser(id, idUser);

            if (deladvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(deladvertisement.Description);

            return BadRequest(deladvertisement.Description);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("ApprovedAdvertisementTrueForAdmin")]
        public async Task<IActionResult> ApprovedAdvertisementTrueForAdmin(long id)
        {
            var deladvertisement = await _advertisementService.CreateConfirmationStatusTrueAdvertisementForAdmin(id);

            if (deladvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(deladvertisement.Description);

            return BadRequest(deladvertisement.Description);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("ApprovedAdvertisementFalseForAdmin")]
        public async Task<IActionResult> ApprovedAdvertisementFalseForAdmin(long id)
        {
            var deladvertisement = await _advertisementService.CreateConfirmationStatusFalseAdvertisementForAdmin(id);

            if (deladvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(deladvertisement.Description);

            return BadRequest(deladvertisement.Description);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("DeleteAdvertisementForAdmin")]
        public async Task<IActionResult> DeleteAdvertisementForAdmin(long id)
        {
            var deladvertisement = await _advertisementService.DeleteAdvertisementForAdmin(id);

            if (deladvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(deladvertisement.Description);

            return BadRequest(deladvertisement.Description);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAdvertisement([FromBody] CreateAdvertisementViewModel model, long id)
        {
            var deladvertisement = await _advertisementService.Edit(model, id);

            if (deladvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(deladvertisement.Description);

            return BadRequest(deladvertisement.Description);
        }

        [HttpGet("GetAdvertisement")]
        public async Task<IActionResult> GetAdvertisement([FromQuery] long id)
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
            model.types = _advertisementService.GetAllObjectTypes();
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
        [HttpPost("CreateAdvertisementModel")]
        public async Task<IActionResult> GetCreateAdvertisementForm([FromQuery] long id)
        {
            var model = await _advertisementService.GetAdvertisementByIdCreateModel(id);
            
            if (model.StatusCode == Domain.Enum.StatusCode.OK)
            {              
                model.Data!.types = _advertisementService.GetAllObjectTypes();
                var amenityTypes = await _amenityService.GetAllAmenityTypes();

                if (amenityTypes.Count == 0)
                {
                    return Ok(model.Data);
                }

                if (model.Data!.CreateAdvertisementAmenities.Count > 0)
                {
                    foreach(var amenityType in amenityTypes)
                    {
                        var element = new CreateAdvertisementAmenityViewModel
                        {
                            Amenity = amenityType,
                            AmenityDisplay = amenityType.GetDisplayName(),
                            IsActive = false,
                            Value = null
                        };

                        if (!model.Data.CreateAdvertisementAmenities.Any(a => a.Amenity == amenityType))
                        {
                            model.Data.CreateAdvertisementAmenities.Add(new CreateAdvertisementAmenityViewModel
                            {
                                Amenity = amenityType,
                                AmenityDisplay = amenityType.GetDisplayName(),
                                IsActive = false,
                                Value = null
                            });
                        }
                    }
                }

                return Ok(model.Data);
            }

            return BadRequest(model.Description);
        }

        [Authorize]
        [HttpPost("CreateAdvertisement")]
        public async Task<IActionResult> CreateAdvertisement([FromBody] CreateAdvertisementViewModel advertisement)
        {
            var newadvertisement = await _advertisementService.CreateAdvertisement(advertisement);

            if(newadvertisement.StatusCode == Domain.Enum.StatusCode.OK) return Ok(newadvertisement.Description);

            return BadRequest(newadvertisement.Description); 
        }
    }
}
