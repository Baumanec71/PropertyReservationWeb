using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.DAL.Repositories;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Extensions;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.Advertisement;
using PropertyReservationWeb.Domain.ViewModels.Convenience;
using PropertyReservationWeb.Domain.ViewModels.Photo;
using PropertyReservationWeb.Domain.ViewModels.User;
using PropertyReservationWeb.Service.Interfaces;
using System.Collections.Concurrent;
using System.Linq;

namespace PropertyReservationWeb.Service.Implementations
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IBaseRepository<Advertisement> _advertisementRepository;
        private readonly IBaseRepository<RentalRequest> _rentalRequestRepository;
        private readonly IBaseRepository<Amenity> _amenityRepository;
        private readonly IBaseRepository<AdvertisementAmenity> _advertisementAmenityRepository;
        private readonly IAdvertisementAmenityRepository _advertisementAmenityRepositoryDop;
        private readonly IPhotoRepository _photoRepositoryDop;
        private readonly IBaseRepository<User> _userRepository;

        private readonly IBaseRepository<Photo> _photoRepository;
        private const int pageSize = 20;
        public AdvertisementService
            (
            IBaseRepository<Advertisement> advertisementRepository, IBaseRepository<RentalRequest> rentalRequestRepository, 
            IBaseRepository<Amenity> amenityRepository, IBaseRepository<AdvertisementAmenity> advertisementAmenityRepository, IBaseRepository<Photo> photoRepository,
            IAdvertisementAmenityRepository advertisementAmenityRepositoryDop, IPhotoRepository photoRepositoryDop,
            IBaseRepository<User> userRepository
            ) 
        {
            _userRepository = userRepository;
            _photoRepositoryDop = photoRepositoryDop;
            _advertisementAmenityRepositoryDop = advertisementAmenityRepositoryDop;
            _photoRepository = photoRepository;
            _amenityRepository = amenityRepository;
            _advertisementAmenityRepository = advertisementAmenityRepository;
            _advertisementRepository = advertisementRepository;
            _rentalRequestRepository = rentalRequestRepository;
        }

        public async Task<IBaseResponse<AdvertisementViewModel>> CalculatingTheRating(long id)
        {
            try
            {
                var currentUtcTime = DateTime.UtcNow;
                var advertisement = await _advertisementRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (advertisement == null)
                {
                    return new BaseResponse<AdvertisementViewModel>()
                    {
                        Description = "Объявление не найдено",
                        StatusCode = StatusCode.AdvertisementNotFound
                    };
                }

                var listIdRentalRequests = await _rentalRequestRepository
                    .GetAll()
                    .Where(r => r.IdNeedAdvertisement == id
                               && r.ApprovalStatus == ApprovalStatus.Approved)
                    .SelectMany(r => r.Review
                        .Where(rew => rew.StatusDel != true
                                && rew.IdNeedRentalRequest == r.Id
                                && rew.IsTheLandlord == false))
                    .ToListAsync();

                advertisement.Rating = listIdRentalRequests.Any()
                    ? Math.Round(listIdRentalRequests.Average(r => r.TheQualityOfTheTransaction), 2)
                    : 0;

                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = "Рейтинг пересчитан",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = $"[CalculatingTheRating]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AdvertisementViewModel>> CreateAdvertisement(CreateAdvertisementViewModel model)
        {
            try
            {
                if (model.ObjectType == ObjectType.Apartment && string.IsNullOrWhiteSpace(model.ApartmentNumber))
                {
                    return new BaseResponse<AdvertisementViewModel>()
                    {
                        Description = "Для объекта типа \"квартира\" номер квартиры обязателен.",
                        StatusCode = StatusCode.InternalServerError,
                    };
                }

                var point = new Point(model.Longitude, model.Latitude) { SRID = 4326 };

                var advertisement = await _advertisementRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x =>
                        x.AdressCoordinates!.Equals(point) &&
                        x.AdressName == model.AdressName &&
                        (model.ObjectType == ObjectType.Apartment ? x.ApartmentNumber == model.ApartmentNumber : true) &&
                        x.DeletionStatus == false);

                if (advertisement != null)
                {
                    return new BaseResponse<AdvertisementViewModel>()
                    {
                        Description = "Объявление с таким адресом и номером квартиры уже существует",
                        StatusCode = StatusCode.InternalServerError,
                    };
                }

                long? userId = await _userRepository
                    .GetAll()
                    .Where(u => u.Email == model.Login)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();

                if (userId == null)
                {
                    return new BaseResponse<AdvertisementViewModel>()
                    {
                        Description = "Такого пользователя нет",
                        StatusCode = StatusCode.UserNotFound,
                    };
                }

                advertisement = new Advertisement(
                    model.ObjectType,
                    point,
                    model.AdressName,
                    model.ApartmentNumber,
                    false,
                    false,
                    model.Description,
                    model.TotalArea,
                    model.RentalPrice,
                    model.FixedPrepaymentAmount,
                    model.NumberOfRooms,
                    model.NumberOfBeds,
                    model.NumberOfBathrooms,
                    DateTime.Now,
                    0,
                    0,
                    (long)userId
                    );

                await _advertisementRepository.Create(advertisement);

                var createAdvertisementAmenitiesList = new List<AdvertisementAmenity>();

                foreach (var advertisementAmenitie in model.CreateAdvertisementAmenities)
                {
                    var amenityId = await _amenityRepository
                        .GetAll()
                        .Where(x => x.AmenityType == advertisementAmenitie.Amenity)
                        .Select(x => (long?)x.Id)
                        .FirstOrDefaultAsync();

                    if (amenityId == null)
                    {
                        continue;
                    }

                    createAdvertisementAmenitiesList.Add(new AdvertisementAmenity(
                        advertisement.Id,
                        amenityId.Value,
                        advertisementAmenitie.IsActive,
                        advertisementAmenitie.Value
                    ));
                }

                if (createAdvertisementAmenitiesList.Count > 0)
                {
                    await _advertisementAmenityRepositoryDop.CreateRange(createAdvertisementAmenitiesList);
                }

                if (model.CreatePhotos.Any())
                {
                    var photos = model.CreatePhotos.Select(photo => new Photo
                    {
                        ValuePhoto = Convert.FromBase64String(photo.ValuePhoto),
                        DeleteStatus = false,
                        IdAdvertisement = advertisement.Id
                    }).ToList();

                    await _photoRepositoryDop.CreateRange(photos);
                }

                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = "Объявление создано",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = $"[CreateAdvertisement]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AdvertisementViewModel>> DeleteAdvertisementForUser(long id)
        {
            try
            {
                var advertisement = await _advertisementRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (advertisement == null)
                {
                    return new BaseResponse<AdvertisementViewModel>()
                    {
                        Description = "Такого объявления нет",
                        StatusCode = StatusCode.AdvertisementNotFound,
                    };
                }

                var rentalrequests = await _rentalRequestRepository
                    .GetAll()
                    .Where(x => x.IdNeedAdvertisement == id && x.BookingFinishDate > DateTime.Now.ToUniversalTime())
                    .ToListAsync();

                rentalrequests.ForEach(async rental =>
                {
                    rental.DeleteStatus = true;
                    rental.ApprovalStatus = ApprovalStatus.Rejected;
                    rental.DataChangeStatus = DateTime.Now;
                    await _rentalRequestRepository.Update(rental);
                });

                advertisement.DeletionStatus = true;
                advertisement.ConfirmationStatus = false;
                await _advertisementRepository.Update(advertisement);

                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = "Объявление удалено",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = $"[DeleteAdvertisementForUser]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AdvertisementViewModel>> Edit(AdvertisementViewModel model)
        {
            try
            {
                var advertisement = await _advertisementRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == model.Id);

                if (advertisement == null)
                {
                    return new BaseResponse<AdvertisementViewModel>
                    {
                        Description = "Объявление не найдено",
                        StatusCode = StatusCode.AdvertisementNotFound
                    };
                }

                if (!string.IsNullOrEmpty(model.ObjectType))
                {
                    var objectTypeEnum = Enum.GetValues(typeof(ObjectType))
                        .Cast<ObjectType>()
                        .FirstOrDefault(e => e.GetDisplayName() == model.ObjectType);

                    if (objectTypeEnum != default) 
                        advertisement.ObjectType = objectTypeEnum;
                }

                advertisement.AdressCoordinates = model.AdressCoordinates ?? advertisement.AdressCoordinates;
                advertisement.NumberOfBathrooms = model.NumberOfBathrooms ?? advertisement.NumberOfBathrooms;
                advertisement.NumberOfBeds = model.NumberOfBeds ?? advertisement.NumberOfBeds;
                advertisement.NumberOfRooms = model.NumberOfRooms ?? advertisement.NumberOfRooms;
                advertisement.AdressName = model.AdressName ?? advertisement.AdressName;
                advertisement.ApartmentNumber = model.ApartmentNumber ?? advertisement.ApartmentNumber;
                advertisement.ConfirmationStatus = model.ConfirmationStatus ?? advertisement.ConfirmationStatus;
                advertisement.Description = model.Description ?? advertisement.Description;
                advertisement.RentalPrice = model.RentalPrice ?? advertisement.RentalPrice;
                advertisement.TotalArea = model.TotalArea ?? advertisement.TotalArea;
                advertisement.FixedPrepaymentAmount = model.FixedPrepaymentAmount ?? advertisement.FixedPrepaymentAmount;
                advertisement.Description = model.Description ?? advertisement.Description;

                await _advertisementRepository.Update(advertisement);

                return new BaseResponse<AdvertisementViewModel>
                {
                    Data = model,
                    Description = "Данные обновлены",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdvertisementViewModel>
                {
                    Description = $"[Edit]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AdvertisementViewModel>> GetAdvertisement(long id)
        {
            try
            {
                var currentUtcTime = DateTime.UtcNow;
                var advertisement = await _advertisementRepository
                    .GetAll()
                    .Include(x => x.Photos)
                    .Include(x => x.AdvertisementAmenities)
                    .ThenInclude(ac => ac.Amenity)
                    .Select(x => new AdvertisementViewModel
                    {
                        Id = x.Id,
                        ObjectType = x.ObjectType.GetDisplayName(),
                        AdressName = x.AdressName,
                        AdressCoordinates = x.AdressCoordinates,
                        ApartmentNumber = x.ApartmentNumber,
                        Description = x.Description,
                        TotalArea = x.TotalArea,
                        RentalPrice = x.RentalPrice,
                        FixedPrepaymentAmount = x.FixedPrepaymentAmount,
                        Rating = x.Rating,
                        NumberOfTransactions = x.RentalRequests.Count(r => r.BookingFinishDate <= currentUtcTime),
                        ConfirmationStatus = x.ConfirmationStatus,
                        DeletionStatus = x.DeletionStatus,
                        DateCreate = x.DateCreate.ToString("yyyy-MM-dd HH:mm:ss"),
                        NumberOfRooms = x.NumberOfRooms,
                        NumberOfBeds = x.NumberOfBeds,
                        NumberOfBathrooms = x.NumberOfBathrooms,
                        IdAuthor = x.IdAuthor,
                        Photos = x.Photos.Where(p => !p.DeleteStatus).Select(p => new PhotoViewModel
                        (
                            p.Id,
                            p.ValuePhoto != null
                            ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                            : string.Empty
                        )
                        ).ToList(),
                        Amenityes = x.AdvertisementAmenities.Where(ac => ac.IsActive == true).Select(ac => new AmenityViewModel
                        (
                            ac.Amenity.Id,
                            ac.Amenity.AmenityType.GetDisplayName(),
                            ac.Value
                        )).ToList()
                    })
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new BaseResponse<AdvertisementViewModel>
                {
                    Data = advertisement,
                    StatusCode = advertisement != null ? StatusCode.OK : StatusCode.AdvertisementNotFound,
                    Description = advertisement != null ? "Ваше объявление" : "Объявление не найдено"
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = $"[GetAdvertisement]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        private async Task<IBaseResponse<List<Advertisement>>> Filter(AdvertisementFilterModel filterModel)
        {
            try
            {
                var query = _advertisementRepository.GetAll()
                    .Include(x => x.Photos)
                    .Include(x => x.AdvertisementAmenities)
                    .ThenInclude(ac => ac.Amenity)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(filterModel.SelectedAddress))
                    query = query.Where(x => x.AdressName.Contains(filterModel.SelectedAddress));

                if (filterModel.SelectedObjectType.HasValue)
                    query = query.Where(x => x.ObjectType == filterModel.SelectedObjectType.Value);

                if (filterModel.SelectedTotalArea.HasValue)
                    query = query.Where(x => x.TotalArea >= filterModel.SelectedTotalArea.Value);

                if (filterModel.SelectedMinRentalPrice.HasValue)
                    query = query.Where(x => x.RentalPrice >= filterModel.SelectedMinRentalPrice.Value);

                if (filterModel.SelectedMaxRentalPrice.HasValue)
                    query = query.Where(x => x.RentalPrice <= filterModel.SelectedMaxRentalPrice.Value);

                if (filterModel.SelectedMinFixedPrepaymentAmount.HasValue)
                    query = query.Where(x => x.FixedPrepaymentAmount >= filterModel.SelectedMinFixedPrepaymentAmount.Value);

                if (filterModel.SelectedMaxFixedPrepaymentAmount.HasValue)
                    query = query.Where(x => x.FixedPrepaymentAmount <= filterModel.SelectedMaxFixedPrepaymentAmount.Value);

                if (filterModel.SelectedNumberOfRooms.HasValue)
                    query = query.Where(x => x.NumberOfRooms == filterModel.SelectedNumberOfRooms.Value);

                if (filterModel.SelectedNumberOfBeds.HasValue)
                    query = query.Where(x => x.NumberOfBeds == filterModel.SelectedNumberOfBeds.Value);

                if (filterModel.SelectedNumberOfBathrooms.HasValue)
                    query = query.Where(x => x.NumberOfBathrooms == filterModel.SelectedNumberOfBathrooms.Value);

                if (filterModel.SelectedMinRating.HasValue)
                    query = query.Where(x => x.Rating >= filterModel.SelectedMinRating.Value);

                if (filterModel.SelectedNumberOfPromotionPoints.HasValue)
                    query = query.Where(x => x.NumberOfPromotionPoints == filterModel.SelectedNumberOfPromotionPoints.Value);

                if (filterModel.SelectedConfirmationStatus.HasValue)
                    query = query.Where(x => x.ConfirmationStatus == filterModel.SelectedConfirmationStatus.Value);

                if (filterModel.CreateAdvertisementAmenities != null && filterModel.CreateAdvertisementAmenities.Where(x=>x.IsActive == true).Any())
                {
                    var amenityValues = filterModel.CreateAdvertisementAmenities.Select(a => a.Amenity).ToList();
                    query = query.Where(x => x.AdvertisementAmenities.Any(ac => amenityValues.Contains(ac.Amenity.AmenityType)));
                }

                var advertisements = await query.ToListAsync();
                return new BaseResponse<List<Advertisement>>()
                {
                    Data = advertisements,
                    Description = "Фильтрация выполнена успешно",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Advertisement>>()
                {
                    Description = $"[Filter]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }


        public async Task<IBaseResponse<PaginatedViewModelResponse<AdvertisementViewModel>>> GetAdvertisements(int page, AdvertisementFilterModel filterModel)
        {
            try
            {
                var filterResult = await Filter(filterModel);
                if (filterResult.StatusCode != StatusCode.OK || filterResult.Data == null || !filterResult.Data.Any())
                {
                    return new BaseResponse<PaginatedViewModelResponse<AdvertisementViewModel>>
                    {
                        Description = "Объявлений по заданным критериям нет",
                        StatusCode = StatusCode.AdvertisementNotFound
                    };
                }

                var totalAdvertisements = filterResult.Data.Count;
                var query = filterResult.Data.AsQueryable().Select(x => new AdvertisementViewModel
                {
                    Id = x.Id,
                    ObjectType = x.ObjectType.GetDisplayName(),
                    AdressName = x.AdressName,
                    AdressCoordinates = x.AdressCoordinates,
                    ApartmentNumber = x.ApartmentNumber,
                    Description = x.Description,
                    TotalArea = x.TotalArea,
                    RentalPrice = x.RentalPrice,
                    FixedPrepaymentAmount = x.FixedPrepaymentAmount,
                    Rating = x.Rating,
                    NumberOfTransactions = x.RentalRequests.Count(r => r.BookingFinishDate <= DateTime.UtcNow),
                    ConfirmationStatus = x.ConfirmationStatus,
                    DeletionStatus = x.DeletionStatus,
                    DateCreate = x.DateCreate.ToString("yyyy-MM-dd HH:mm:ss"),
                    NumberOfRooms = x.NumberOfRooms,
                    NumberOfBeds = x.NumberOfBeds,
                    NumberOfBathrooms = x.NumberOfBathrooms,
                    IdAuthor = x.IdAuthor,
                    Photos = x.Photos.Select(p => new PhotoViewModel
                    (
                        p.Id,
                        p.ValuePhoto != null
                        ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                        : string.Empty
                    )).ToList(),
                    Amenityes = x.AdvertisementAmenities.Select(ac => new AmenityViewModel
                    (
                        ac.Amenity.Id,
                        ac.Amenity.AmenityType.GetDisplayName(),
                        ac.Value
                    )).ToList()
                });

                var totalPages = (int)Math.Ceiling((double)totalAdvertisements / pageSize);
                var advertisements = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginatedViewModelResponse<AdvertisementViewModel>
                (
                    advertisements,
                    totalPages,
                    filterModel
                );

                return new BaseResponse<PaginatedViewModelResponse<AdvertisementViewModel>>
                {
                    Data = response,
                    StatusCode = advertisements.Any() ? StatusCode.OK : StatusCode.AdvertisementNotFound,
                    Description = advertisements.Any() ? "Успешно получены объявления" : "Найдено 0 элементов"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginatedViewModelResponse<AdvertisementViewModel>>
                {
                    Description = $"[GetAdvertisements]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<List<ObjectTypeOptionViewModel>> GetAllObjectTypes()
        {
            var objecttypes = Enum.GetValues(typeof(ObjectType))
                .Cast<ObjectType>()
                .Select(o => new ObjectTypeOptionViewModel(o, o.GetDisplayName()))
                .ToList();
            return objecttypes;
        }

        public async Task<IBaseResponse<List<AdvertisementViewModel>>> GetConfirmationAdvertisements(int page)
        {
            try
            {
                var currentUtcTime = DateTime.UtcNow;
                var advertisements = await _advertisementRepository
                    .GetAll()
                    .Where(x => x.ConfirmationStatus && !x.DeletionStatus)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.Photos)
                    .Include(x => x.AdvertisementAmenities)
                    .ThenInclude(ac => ac.Amenity)
                    .AsNoTracking()
                    .Select(x => new AdvertisementViewModel
                    {
                        Id = x.Id,
                        ObjectType = x.ObjectType.GetDisplayName(),
                        AdressName = x.AdressName,
                        AdressCoordinates = x.AdressCoordinates,
                        ApartmentNumber = x.ApartmentNumber,
                        Description = x.Description,
                        TotalArea = x.TotalArea,
                        RentalPrice = x.RentalPrice,
                        FixedPrepaymentAmount = x.FixedPrepaymentAmount,
                        Rating = x.Rating,
                        NumberOfTransactions = x.RentalRequests.Count(r => r.BookingFinishDate <= currentUtcTime),
                        ConfirmationStatus = x.ConfirmationStatus,
                        DeletionStatus = x.DeletionStatus,
                        DateCreate = x.DateCreate.ToString("yyyy-MM-dd HH:mm:ss"),
                        NumberOfRooms = x.NumberOfRooms,
                        NumberOfBeds = x.NumberOfBeds,
                        NumberOfBathrooms = x.NumberOfBathrooms,
                        IdAuthor = x.IdAuthor,
                        Photos = x.Photos.Where(p => !p.DeleteStatus).Select(p => new PhotoViewModel
                        (
                            p.Id,
                            p.ValuePhoto != null
                            ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                            : string.Empty
                        )).ToList(),
                        Amenityes = x.AdvertisementAmenities.Where(ac=>ac.IsActive == true).Select(ac => new AmenityViewModel
                        (
                            ac.Amenity.Id,
                            ac.Amenity.AmenityType.GetDisplayName(),
                            ac.Value
                        )).ToList()
                    })
                    .ToListAsync();

                return new BaseResponse<List<AdvertisementViewModel>>
                {
                    Data = advertisements,
                    StatusCode = advertisements.Any() ? StatusCode.OK : StatusCode.AdvertisementNotFound,
                    Description = advertisements.Any() ? "Успешно получены объявления" : "Найдено 0 элементов"
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AdvertisementViewModel>>()
                {
                    Description = $"[GetConfirmationAdvertisements]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<AdvertisementViewModel>>> GetMyAdvertisements(long id, int page)
        {
            try
            {
                var currentUtcTime = DateTime.UtcNow;
                var advertisements = await _advertisementRepository
                    .GetAll()
                    .Where(x => x.IdAuthor == id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.Photos)
                    .Include(x => x.AdvertisementAmenities)
                    .ThenInclude(ac => ac.Amenity)
                    .AsNoTracking()
                    .Select(x => new AdvertisementViewModel
                    {
                        Id = x.Id,
                        ObjectType = x.ObjectType.GetDisplayName(),
                        AdressName = x.AdressName,
                        AdressCoordinates = x.AdressCoordinates,
                        ApartmentNumber = x.ApartmentNumber,
                        Description = x.Description,
                        TotalArea = x.TotalArea,
                        RentalPrice = x.RentalPrice,
                        FixedPrepaymentAmount = x.FixedPrepaymentAmount,
                        Rating = x.Rating,
                        NumberOfTransactions = x.RentalRequests.Count(r => r.BookingFinishDate <= currentUtcTime),
                        ConfirmationStatus = x.ConfirmationStatus,
                        DeletionStatus = x.DeletionStatus,
                        DateCreate = x.DateCreate.ToString("yyyy-MM-dd HH:mm:ss"),
                        NumberOfRooms = x.NumberOfRooms,
                        NumberOfBeds = x.NumberOfBeds,
                        NumberOfBathrooms = x.NumberOfBathrooms,
                        IdAuthor = x.IdAuthor,
                        Photos = x.Photos.Where(p => !p.DeleteStatus).Select(p => new PhotoViewModel
                        (
                            p.Id,
                            p.ValuePhoto != null
                            ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                            : string.Empty
                        )).ToList(),
                        Amenityes = x.AdvertisementAmenities.Where(ac => ac.IsActive == true).Select(ac => new AmenityViewModel
                        (
                            ac.Amenity.Id,
                            ac.Amenity.AmenityType.GetDisplayName(),
                            ac.Value
                        )).ToList()
                    })
                    .ToListAsync();

                return new BaseResponse<List<AdvertisementViewModel>>
                {
                    Data = advertisements,
                    StatusCode = advertisements.Any() ? StatusCode.OK : StatusCode.AdvertisementNotFound,
                    Description = advertisements.Any() ? "Успешно получены объявления" : "Найдено 0 элементов"
                };


            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AdvertisementViewModel>>()
                {
                    Description = $"[GetMyAdvertisements]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<AdvertisementViewModel>>> GetMyNoDeleteAdvertisements(long id, int page)
        {
            try
            {
                var currentUtcTime = DateTime.UtcNow;
                var advertisements = await _advertisementRepository
                    .GetAll()
                    .Where(x => x.IdAuthor == id && x.DeletionStatus != true)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.Photos)
                    .Include(x => x.AdvertisementAmenities)
                    .ThenInclude(ac => ac.Amenity)
                    .AsNoTracking()
                    .Select(x => new AdvertisementViewModel
                    {
                        Id = x.Id,
                        ObjectType = x.ObjectType.GetDisplayName(),
                        AdressName = x.AdressName,
                        AdressCoordinates = x.AdressCoordinates,
                        Description = x.Description,
                        ApartmentNumber = x.ApartmentNumber,
                        TotalArea = x.TotalArea,
                        RentalPrice = x.RentalPrice,
                        FixedPrepaymentAmount = x.FixedPrepaymentAmount,
                        Rating = x.Rating,
                        NumberOfTransactions = x.RentalRequests.Count(r => r.BookingFinishDate <= currentUtcTime),
                        ConfirmationStatus = x.ConfirmationStatus,
                        DeletionStatus = x.DeletionStatus,
                        DateCreate = x.DateCreate.ToString("yyyy-MM-dd HH:mm:ss"),
                        NumberOfRooms = x.NumberOfRooms,
                        NumberOfBeds = x.NumberOfBeds,
                        NumberOfBathrooms = x.NumberOfBathrooms,
                        IdAuthor = x.IdAuthor,
                        Photos = x.Photos.Where(p => !p.DeleteStatus).Select(p => new PhotoViewModel
                        (
                            p.Id,
                            p.ValuePhoto != null
                            ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                            : string.Empty
                        )).ToList(),
                        Amenityes = x.AdvertisementAmenities.Where(ac => ac.IsActive == true).Select(ac => new AmenityViewModel
                        (
                            ac.Amenity.Id,
                            ac.Amenity.AmenityType.GetDisplayName(),
                            ac.Value
                        )).ToList()
                    })
                    .ToListAsync();

                return new BaseResponse<List<AdvertisementViewModel>>
                {
                    Data = advertisements,
                    StatusCode = advertisements.Any() ? StatusCode.OK : StatusCode.AdvertisementNotFound,
                    Description = advertisements.Any() ? "Успешно получены объявления" : "Найдено 0 элементов"
                };


            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AdvertisementViewModel>>()
                {
                    Description = $"[GetMyAdvertisements]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
