using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Extensions;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.Advertisement;
using PropertyReservationWeb.Domain.ViewModels.Amenity;
using PropertyReservationWeb.Domain.ViewModels.Convenience;
using PropertyReservationWeb.Domain.ViewModels.Photo;
using PropertyReservationWeb.Service.Interfaces;
using Point = NetTopologySuite.Geometries.Point;

namespace PropertyReservationWeb.Service.Implementations
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IBaseRepository<Advertisement> _advertisementRepository;
        private readonly IBaseRepository<RentalRequest> _rentalRequestRepository;
        private readonly IBaseRepository<Amenity> _amenityRepository;
        private readonly IBaseRepository<ApprovalRequest> _approvalRequestRepository;
        private readonly IAdvertisementAmenityRepository _advertisementAmenityRepositoryDop;
        private readonly IPhotoRepository<Photo> _photoRepositoryDop;
        private readonly IBaseRepository<Review> _reviewRepository;
        private readonly IBaseRepository<User> _userRepository;

        private const int pageSize = 21;
        public AdvertisementService
        (
            IBaseRepository<Advertisement> advertisementRepository, IBaseRepository<RentalRequest> rentalRequestRepository, 
            IBaseRepository<Amenity> amenityRepository, IBaseRepository<ApprovalRequest> approvalRequestRepository,
            IAdvertisementAmenityRepository advertisementAmenityRepositoryDop, IPhotoRepository<Photo> photoRepositoryDop,
            IBaseRepository<User> userRepository, IBaseRepository<Review> reviewRepository
        ) 
        {
            _reviewRepository = reviewRepository;
            _approvalRequestRepository = approvalRequestRepository;
            _userRepository = userRepository;
            _photoRepositoryDop = photoRepositoryDop;
            _advertisementAmenityRepositoryDop = advertisementAmenityRepositoryDop;
            _amenityRepository = amenityRepository;
            _advertisementRepository = advertisementRepository;
            _rentalRequestRepository = rentalRequestRepository;
        }

        public async Task<IBaseResponse<AdvertisementViewModel>> CalculatingTheRating(long advertisementId)
        {
            try
            {
                var advertisement = await _advertisementRepository
                    .GetAll()
                    .FirstOrDefaultAsync(a => a.Id == advertisementId);

                if (advertisement == null)
                {
                    return new BaseResponse<AdvertisementViewModel>
                    {
                        Description = "Объявление не найдено",
                        StatusCode = StatusCode.AdvertisementNotFound
                    };
                }

                var reviews = await _reviewRepository
                    .GetAll()
                    .AsNoTracking()
                    .Where(r => r.StatusDel == false && r.IsTheLandlord == false)
                    .Join(
                        _rentalRequestRepository.GetAll().AsNoTracking(),
                        review => review.IdNeedRentalRequest,
                        rentalRequest => rentalRequest.Id,
                        (review, rentalRequest) => new { review, rentalRequest }
                    )
                    .Where(x => x.rentalRequest.IdNeedAdvertisement == advertisementId)
                    .Select(x => x.review)
                    .ToListAsync();

                advertisement.Rating = reviews.Any()
                    ? Math.Round(reviews.Average(r => r.TheQualityOfTheTransaction), 2)
                    : 0;

                await _advertisementRepository.Update(advertisement);

                return new BaseResponse<AdvertisementViewModel>
                {
                    Description = "Рейтинг объявления подсчитан",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdvertisementViewModel>
                {
                    Description = $"Ошибка при расчете рейтинга объявления: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<CreateAdvertisementViewModel>> GetAdvertisementByIdCreateModel(long id)
        {
            try
            {
                var advertisement = await _advertisementRepository
                    .GetAll()
                    .Include(x => x.Photos.Where(ph => ph.DeleteStatus == false))
                    .Include(x => x.AdvertisementAmenities.Where(aa => aa.IsActive == true))
                    .ThenInclude(aa => aa.Amenity)
                    .FirstOrDefaultAsync(a => a.Id == id && !a.DeletionStatus);

                if (advertisement == null)
                {
                    return new BaseResponse<CreateAdvertisementViewModel>()
                    {
                        Description = $"Объявление не найдено",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                var viewModel = new CreateAdvertisementViewModel
                {
                    ObjectType = advertisement.ObjectType,
                    AdressName = advertisement.AdressName,
                    ApartmentNumber = advertisement.ApartmentNumber ?? string.Empty,
                    Latitude = advertisement.AdressCoordinates!.Y,
                    Longitude = advertisement.AdressCoordinates.X,
                    Description = advertisement.Description,
                    TotalArea = advertisement.TotalArea,
                    RentalPrice = advertisement.RentalPrice,
                    NumberOfRooms = advertisement.NumberOfRooms,
                    NumberOfBeds = advertisement.NumberOfBeds,
                    NumberOfBathrooms = advertisement.NumberOfBathrooms,
                    CreatePhotos = advertisement.Photos.Select(p => new CreatePhotoViewModel
                    {
                        ValuePhoto = Convert.ToBase64String(p.ValuePhoto),
                        DeleteStatus = p.DeleteStatus
                    }).ToList(),
                    CreateAdvertisementAmenities = advertisement.AdvertisementAmenities.Select(aa => new CreateAdvertisementAmenityViewModel
                    {
                        Amenity = aa.Amenity.AmenityType,
                        AmenityDisplay = aa.Amenity.AmenityType.GetDisplayName(),
                        IsActive = aa.IsActive,
                        Value = aa.Value
                    }).ToList()
                };

                return new BaseResponse<CreateAdvertisementViewModel>()
                {
                    Data = viewModel,
                    Description = "Объявление найдено",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateAdvertisementViewModel>()
                {
                    Description = $"[CreateAdvertisement]:{ex.Message}",
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

                await _approvalRequestRepository.Create(new ApprovalRequest()
                {
                    IdAdvertisement = advertisement.Id,
                    IdUserAdmin = 1,
                    Status = ApprovalStatus.UnderСonsideration,
                    DateChange = DateTime.UtcNow,
                    DateCreate = DateTime.UtcNow,
                });

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

        public async Task<IBaseResponse<CreateAdvertisementViewModel>> Edit(CreateAdvertisementViewModel model, long id)
        {
            try
            {
                var advertisement = await _advertisementRepository
                    .GetAll()
                    .Include(x => x.Photos.Where(ph => ph.DeleteStatus == false))
                    .Include(x => x.AdvertisementAmenities.Where(aa => aa.IsActive == true))
                        .ThenInclude(aa => aa.Amenity)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (advertisement == null)
                {
                    return new BaseResponse<CreateAdvertisementViewModel>
                    {
                        Description = "Объявление не найдено",
                        StatusCode = StatusCode.AdvertisementNotFound
                    };
                }

                bool isUpdated = false;

                if (model.Longitude != 0 && model.Latitude != 0 &&
                    (advertisement.AdressCoordinates == null ||
                     advertisement.AdressCoordinates.X != model.Longitude ||
                     advertisement.AdressCoordinates.Y != model.Latitude))
                {
                    advertisement.AdressCoordinates = new Point(model.Longitude, model.Latitude) { SRID = 4326 };
                    isUpdated = true;
                }

                if (!string.Equals(model.AdressName, advertisement.AdressName, StringComparison.OrdinalIgnoreCase))
                {
                    advertisement.AdressName = model.AdressName;
                    isUpdated = true;
                }

                if (!string.Equals(model.ApartmentNumber, advertisement.ApartmentNumber, StringComparison.OrdinalIgnoreCase))
                {
                    advertisement.ApartmentNumber = model.ApartmentNumber;
                    isUpdated = true;
                }

                if (!string.Equals(model.Description, advertisement.Description, StringComparison.OrdinalIgnoreCase))
                {
                    advertisement.Description = model.Description;
                    isUpdated = true;
                }

                if (model.NumberOfBathrooms != advertisement.NumberOfBathrooms)
                {
                    advertisement.NumberOfBathrooms = model.NumberOfBathrooms;
                    isUpdated = true;
                }

                if (model.NumberOfBeds != advertisement.NumberOfBeds)
                {
                    advertisement.NumberOfBeds = model.NumberOfBeds;
                    isUpdated = true;
                }

                if (model.NumberOfRooms != advertisement.NumberOfRooms)
                {
                    advertisement.NumberOfRooms = model.NumberOfRooms;
                    isUpdated = true;
                }

                if (model.RentalPrice != advertisement.RentalPrice)
                {
                    advertisement.RentalPrice = model.RentalPrice;
                    isUpdated = true;
                }

                if (model.TotalArea != advertisement.TotalArea)
                {
                    advertisement.TotalArea = model.TotalArea;
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    advertisement.ConfirmationStatus = false;
                    await _advertisementRepository.Update(advertisement);

                    await _approvalRequestRepository.Create(new ApprovalRequest()
                    {
                        IdAdvertisement = id,
                        IdUserAdmin = 1,
                        Status = ApprovalStatus.UnderСonsideration,
                        DateChange = DateTime.UtcNow,
                        DateCreate = DateTime.UtcNow,
                    });
                }

                var updatedAmenities = new List<AdvertisementAmenity>();
                var createAdvertisementAmenitiesList = new List<AdvertisementAmenity>();

                foreach (var modelAmenity in model.CreateAdvertisementAmenities)
                {
                    var existingAmenity = advertisement.AdvertisementAmenities
                        .FirstOrDefault(a => (AmenityType)a.Amenity.AmenityType == (AmenityType)modelAmenity.Amenity);

                    if (existingAmenity != null)
                    {
                        if (existingAmenity.IsActive != modelAmenity.IsActive || existingAmenity.Value != modelAmenity.Value)
                        {
                            existingAmenity.IsActive = modelAmenity.IsActive;
                            existingAmenity.Value = modelAmenity.Value;
                            updatedAmenities.Add(existingAmenity);
                        }
                    }

                    else if(modelAmenity.IsActive == true)
                    {
                        var amenityId = await _amenityRepository
                            .GetAll()
                            .Where(x => x.AmenityType == modelAmenity.Amenity)
                            .Select(x => (long?)x.Id)
                            .FirstOrDefaultAsync();

                        if (amenityId == null)
                        {
                            continue;
                        }

                        createAdvertisementAmenitiesList.Add(new AdvertisementAmenity(
                            advertisement.Id,
                            amenityId.Value,
                            modelAmenity.IsActive,
                            modelAmenity.Value
                            ));
                    }
                }

                if (updatedAmenities.Count > 0)
                {
                    await _advertisementAmenityRepositoryDop.UpdateRange(updatedAmenities);
                }

                if (createAdvertisementAmenitiesList.Count > 0)
                {
                    await _advertisementAmenityRepositoryDop.CreateRange(createAdvertisementAmenitiesList);
                }

                if (model.CreatePhotos.Count > 0)
                {
                    var updatedPhotos = new List<Photo>();
                    var createdPhotos = new List<Photo>();
                    List<Photo> yeasornoPhotos = new List<Photo>(advertisement.Photos);

                    foreach (var modelPhoto in model.CreatePhotos)
                    {
                        var existingPhoto = yeasornoPhotos
                            .FirstOrDefault(p => p.ValuePhoto.SequenceEqual(Convert.FromBase64String(modelPhoto.ValuePhoto)));

                        if (existingPhoto != null)
                        {
                            yeasornoPhotos.Remove(existingPhoto);
                        }
                        else
                        {
                            createdPhotos.Add(new Photo
                            {
                                ValuePhoto = Convert.FromBase64String(modelPhoto.ValuePhoto),
                                DeleteStatus = false,
                                IdAdvertisement = advertisement.Id
                            });
                        }
                    }

                    if (yeasornoPhotos.Count > 0)
                    {
                        foreach (var photo in yeasornoPhotos)
                        {
                            photo.DeleteStatus = true;
                            updatedPhotos.Add(photo);
                        }
                    }

                    if (updatedPhotos.Count > 0)
                    {
                        await _photoRepositoryDop.UpdateRange(updatedPhotos);
                    }

                    if (createdPhotos.Count > 0)
                    {
                        await _photoRepositoryDop.CreateRange(createdPhotos);
                    }
                }

                return new BaseResponse<CreateAdvertisementViewModel>
                {
                    Data = model,
                    Description = "Данные обновлены",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateAdvertisementViewModel>
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
                        Rating = x.Rating,
                        NumberOfTransactions = x.RentalRequests.Count(r => r.BookingFinishDate <= currentUtcTime),
                        ConfirmationStatus = x.ConfirmationStatus,
                        DeletionStatus = x.DeletionStatus,
                        DateCreate = x.DateCreate.ToString("yyyy-MM-dd HH:mm:ss"),
                        NumberOfRooms = x.NumberOfRooms,
                        NumberOfBeds = x.NumberOfBeds,
                        NumberOfBathrooms = x.NumberOfBathrooms,
                        IdAuthor = x.IdAuthor,
                        Photos = x.Photos.Where(ph => ph.DeleteStatus == false).Select(p => new PhotoViewModel
                        (
                            p.Id,
                            p.ValuePhoto != null
                            ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                            : string.Empty
                        )).ToList(),
                        Amenityes = x.AdvertisementAmenities
                     .Where(ac => ac.IsActive == true)
                     .Select(ac => new AmenityViewModel(
                         ac.Amenity.Id,
                         ac.Amenity.AmenityType.GetDisplayName(),
                         ac.Value
                         ))
                     .ToList()
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

        private async Task<IBaseResponse<(List<T>, int)>> Filter<T>(int page, AdvertisementFilterModel filterModel)
        {
            try
            {
                var query = _advertisementRepository.GetAll().AsNoTracking()
                    .Include(x => x.Photos)
                    .Include(x => x.AdvertisementAmenities)
                    .ThenInclude(ac => ac.Amenity)
                    .AsSplitQuery()
                    .AsQueryable();

                query = query
                    .OrderByDescending(x => x.NumberOfPromotionPoints)
                    .ThenByDescending(x => x.DateCreate);

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

                if (filterModel.IdAuthor.HasValue)
                    query = query.Where(x => x.IdAuthor == filterModel.IdAuthor.Value);

                if (filterModel.SelectedDeleteStatus.HasValue)
                    query = query.Where(x => x.DeletionStatus == filterModel.SelectedDeleteStatus.Value);

                if (filterModel.CreateAdvertisementAmenities != null && filterModel.CreateAdvertisementAmenities.Any(x => x.IsActive))
                {
                    var requiredAmenityTypes = filterModel.CreateAdvertisementAmenities
                        .Where(x => x.IsActive)
                        .Select(a => a.Amenity)
                        .ToList();

                    query = query.Where(ad =>
                        requiredAmenityTypes.All(requiredType =>
                            ad.AdvertisementAmenities.Any(adAmenity =>
                                adAmenity.IsActive && adAmenity.Amenity.AmenityType == requiredType
                            )
                        )
                    );
                }

                var skip = (page - 1) * pageSize;
                var totalCount = await query.CountAsync();

                if (totalCount == 0)
                {
                    return new BaseResponse<(List<T>, int)>()
                    {
                        Description = "Объявлений по заданным критериям нет",
                        StatusCode = StatusCode.AdvertisementNotFound
                    };
                }

                totalCount++;
                List<T> advertisements = new List<T>();

                if (typeof(T) == typeof(AdvertisementViewModel))
                {
                    var result  = await query
                 .Skip(skip)
                 .Take(pageSize).AsNoTracking().Select(x => new AdvertisementViewModel
                 {
                     Id = x.Id,
                     ObjectType = x.ObjectType.GetDisplayName(),
                     AdressName = x.AdressName,
                     AdressCoordinates = x.AdressCoordinates,
                     ApartmentNumber = x.ApartmentNumber,
                     Description = x.Description,
                     TotalArea = x.TotalArea,
                     RentalPrice = x.RentalPrice,
                     Rating = x.Rating,
                     NumberOfTransactions = x.RentalRequests.Count(r => r.BookingFinishDate <= DateTime.UtcNow),
                     ConfirmationStatus = x.ConfirmationStatus,
                     DeletionStatus = x.DeletionStatus,
                     DateCreate = x.DateCreate.ToString("yyyy-MM-dd HH:mm:ss"),
                     NumberOfRooms = x.NumberOfRooms,
                     NumberOfBeds = x.NumberOfBeds,
                     NumberOfBathrooms = x.NumberOfBathrooms,
                     IdAuthor = x.IdAuthor,
                     Photos = x.Photos.Where(ph => ph.DeleteStatus == false).Select(p => new PhotoViewModel
                     (
                         p.Id,
                         p.ValuePhoto != null
                         ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                         : string.Empty
                     )).ToList(),
                     Amenityes = x.AdvertisementAmenities
                     .Where(ac => ac.IsActive)
                     .Select(ac => new AmenityViewModel(
                         ac.Amenity.Id,
                         ac.Amenity.AmenityType.GetDisplayName(),
                         ac.Value
                         ))
                     .ToList()
                 }).ToListAsync();
                    advertisements = result.Cast<T>().ToList();
                }
                else if (typeof(T) == typeof(ShortAdvertisementViewModel))
                {
                    var result = await query
                        .Skip(skip)
                        .Take(pageSize)
                        .AsNoTracking()
                        .Select(x => new ShortAdvertisementViewModel
                        (
                            x.Id, x.ObjectType.GetDisplayName(), x.AdressName,
                            x.Description, x.TotalArea, x.RentalPrice, x.Rating,
                            x.RentalRequests.Count(r => r.BookingFinishDate <= DateTime.UtcNow),
                            x.NumberOfRooms, x.NumberOfBeds, x.NumberOfBathrooms, x.IdAuthor,
                            x.Photos.Where(ph => ph.DeleteStatus == false).Select(p => new PhotoViewModel
                                         (
                                             p.Id,
                                             p.ValuePhoto != null
                                             ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                                             : string.Empty
                                         )).ToList(),
                            x.AdvertisementAmenities
                            .Where(ac => ac.IsActive)
                            .Select(ac => new AmenityViewModel(
                                ac.Amenity.Id,
                                ac.Amenity.AmenityType.GetDisplayName(),
                                ac.Value
                                ))
                            .ToList()
                            )).ToListAsync();

                    advertisements = result.Cast<T>().ToList();
                }

                return new BaseResponse<(List<T>, int)>()
                {
                    Data = (advertisements, totalCount),
                    Description = "Фильтрация выполнена успешно",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<(List<T>, int)>()
                {
                    Description = $"[Filter]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<PaginatedViewModelResponse<T, AdvertisementFilterModel>>> GetAdvertisements<T>(int page, AdvertisementFilterModel filterModel, bool? selectedConfirmationStatus, long? idAuthor, bool? selectedDeleteStatus)
        {
            try
            {
                if (selectedConfirmationStatus != null)
                {
                    filterModel.SelectedConfirmationStatus = selectedConfirmationStatus;
                }

                if (idAuthor != null)
                {
                    filterModel.IdAuthor = idAuthor;
                }

                if (selectedDeleteStatus != null)
                {
                    filterModel.SelectedDeleteStatus = selectedDeleteStatus;
                }

                var filterResult = await Filter<T>(page, filterModel);

                if (filterResult.StatusCode != StatusCode.OK)
                {
                    return new BaseResponse<PaginatedViewModelResponse<T, AdvertisementFilterModel>>
                    {
                        Description = filterResult.Description,
                        StatusCode = filterResult.StatusCode
                    };
                }

                var response = new PaginatedViewModelResponse<T, AdvertisementFilterModel>
                (
                    filterResult.Data.Item1,
                    (int)(filterResult.Data.Item2 / pageSize),
                    filterModel
                );

                return new BaseResponse<PaginatedViewModelResponse<T, AdvertisementFilterModel>>
                {
                    Data = response,
                    StatusCode = StatusCode.OK,
                    Description = "Успешно получены объявления"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginatedViewModelResponse<T, AdvertisementFilterModel>>
                {
                    Description = $"[GetAdvertisements]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AdvertisementViewModel>> DeleteAdvertisementForUser(long id, long idUser)
        {
            try
            {
                var advertisement = await _advertisementRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id && x.IdAuthor == idUser);

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

                if (rentalrequests.Count > 0)
                {
                    rentalrequests.ForEach(async rental =>
                    {
                        rental.DeleteStatus = true;
                        rental.ApprovalStatus = ApprovalStatus.Rejected;
                        rental.DataChangeStatus = DateTime.Now;
                        await _rentalRequestRepository.Update(rental);
                    });
                }

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

        public async Task<IBaseResponse<AdvertisementViewModel>> DeleteAdvertisementForAdmin(long id)
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

                if (rentalrequests.Count > 0)
                {
                    rentalrequests.ForEach(async rental =>
                    {
                        rental.DeleteStatus = true;
                        rental.ApprovalStatus = ApprovalStatus.Rejected;
                        rental.DataChangeStatus = DateTime.Now;
                        await _rentalRequestRepository.Update(rental);
                    });
                }

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

        public async Task<IBaseResponse<AdvertisementViewModel>> CreateConfirmationStatusTrueAdvertisementForAdmin(long id)
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

                advertisement.DeletionStatus = false;
                advertisement.ConfirmationStatus = true;
                await _advertisementRepository.Update(advertisement);

                var approvedstatus = await _approvalRequestRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.IdAdvertisement == id);

                if (approvedstatus != null)
                {
                    approvedstatus.DateChange = DateTime.UtcNow;
                    approvedstatus.Status = ApprovalStatus.Approved;
                    await _approvalRequestRepository.Update(approvedstatus);
                }
                else
                {
                    return new BaseResponse<AdvertisementViewModel>()
                    {
                        Description = "Не найден запрос на одобрение",
                        StatusCode = StatusCode.ApprovalRequestNotFound,
                    };
                }

                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = "Объявление Одобрено",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = $"[CreateConfirmationStatusTrueAdvertisementForAdmin]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AdvertisementViewModel>> CreateConfirmationStatusFalseAdvertisementForAdmin(long id)
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

                var approvedstatus = await _approvalRequestRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.IdAdvertisement == id);


                advertisement.ConfirmationStatus = false;
                await _advertisementRepository.Update(advertisement);

                if (approvedstatus != null)
                {
                    approvedstatus.DateChange = DateTime.UtcNow;
                    approvedstatus.Status = ApprovalStatus.Rejected;
                    await _approvalRequestRepository.Update(approvedstatus);
                }
                else
                {
                    return new BaseResponse<AdvertisementViewModel>()
                    {
                        Description = "Не найден запрос на одобрение",
                        StatusCode = StatusCode.ApprovalRequestNotFound,
                    };
                }

                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = "Объявление Отклонено",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdvertisementViewModel>()
                {
                    Description = $"[CreateConfirmationStatusFalseAdvertisementForAdmin]:{ex.Message}",
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
    }
}
