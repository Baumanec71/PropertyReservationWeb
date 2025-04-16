using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.BookingPhoto;
using PropertyReservationWeb.Service.Interfaces;

namespace PropertyReservationWeb.Service.Implementations
{
    public class BookingPhotoService : IBookingPhotoService
    {
        private readonly IBaseRepository<BookingPhoto> _photoRepository;
        private readonly IBaseRepository<RentalRequest> _rentalRequestRepository;
        private readonly IBaseRepository<User> _userRepository;

        IPhotoRepository<BookingPhoto> _photoRepositorydop;

        public BookingPhotoService(IBaseRepository<BookingPhoto> photoRepository, IBaseRepository<RentalRequest> rentalRequestRepository, IBaseRepository<User> userRepository, IPhotoRepository<BookingPhoto> photoRepositorydop)
        {
            _userRepository = userRepository;
            _photoRepository = photoRepository;
            _rentalRequestRepository = rentalRequestRepository;
            _photoRepositorydop = photoRepositorydop;
        }
        public async Task<IBaseResponse<List<CreateBookingPhotoViewModel>>> AddPhotoAsync(List<CreateBookingPhotoViewModel> photos, long rentalRequestId, long idUser)
        {
            try
            {
                var rentalRequest = await _rentalRequestRepository
                    .GetAll()
                    .Include(rr=>rr.Advertisement)
                    .FirstOrDefaultAsync(rr => rr.Id == rentalRequestId);

                if (rentalRequest == null)
                {
                    return new BaseResponse<List<CreateBookingPhotoViewModel>>()
                    {
                        Description = "Запрос на аренду не найден",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }

                var user = await _userRepository
                    .GetAll()
                    .FirstOrDefaultAsync(rr => rr.Id == idUser && rr.Status != true);

                if (user == null)
                {
                    return new BaseResponse<List<CreateBookingPhotoViewModel>>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound,
                    };
                }

                var timeToCheckIn = (rentalRequest.BookingStartDate + rentalRequest.CheckInTime - DateTime.UtcNow).TotalHours;

                if (rentalRequest.Advertisement.IdAuthor == idUser)
                {
                    await _photoRepositorydop.CreateRange(photos.Select(photo => new BookingPhoto
                    {
                        ValuePhoto = Convert.FromBase64String(photo.ValuePhoto),
                        DeleteStatus = false,
                        IdRentalRequest = rentalRequestId,
                        DateCreate = photo.DateCreate.ToUniversalTime(),
                        Before = true,
                    }).ToList());

                    rentalRequest.IsBeforePhotosUploaded = true;
                    await _rentalRequestRepository.Update(rentalRequest);

                    return new BaseResponse<List<CreateBookingPhotoViewModel>>()
                    {
                        Description = "Фотографии оставлены успешно",
                        StatusCode = StatusCode.OK,
                    };
                }
                else if(rentalRequest.IdAuthorRentalRequest == idUser)
                {
                    await _photoRepositorydop.CreateRange(photos.Select(photo => new BookingPhoto
                    {
                        ValuePhoto = Convert.FromBase64String(photo.ValuePhoto),
                        DeleteStatus = false,
                        IdRentalRequest = rentalRequestId,
                        DateCreate = photo.DateCreate.ToUniversalTime(),
                        Before = false,
                    }).ToList());

                    rentalRequest.IsAfterPhotosUploaded = true;
                    await _rentalRequestRepository.Update(rentalRequest);

                    return new BaseResponse<List<CreateBookingPhotoViewModel>>()
                    {
                        Description = "Фотографии оставлены успешно",
                        StatusCode = StatusCode.OK,
                    };
                }         

                return new BaseResponse<List<CreateBookingPhotoViewModel>>()
                {
                    Description = "Вы не можете оставить фотографии",
                    StatusCode = StatusCode.CreateBookingPhotoError,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<CreateBookingPhotoViewModel>>()
                {
                    Description = $"Произошла непредвиденная ошибка {ex}",
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<(List<BookingPhotoViewModel>, List<BookingPhotoViewModel>)>> GetPhotosAsync(long rentalRequestId, bool? isBefore = null)
        {
            try
            {
                var query = _photoRepository
                    .GetAll()
                    .Where(p => p.IdRentalRequest == rentalRequestId && !p.DeleteStatus);

                if (isBefore != null)
                {
                    query = query.Where(p => p.Before == isBefore);
                }

                var photos = await query.ToListAsync();

                var result = photos
                    .Select(p => new BookingPhotoViewModel(
                        p.Id,
                        p.ValuePhoto != null
                        ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                        : string.Empty,
                        p.Before,
                        p.DateCreate.ToLocalTime()
                    )).Where(p => p.IsBefore == true)
                    .ToList();

                var result2 = photos
                    .Select(p => new BookingPhotoViewModel(
                        p.Id,
                        p.ValuePhoto != null
                        ? $"data:image/png;base64,{Convert.ToBase64String(p.ValuePhoto)}"
                        : string.Empty,
                        p.Before,
                        p.DateCreate.ToLocalTime()
                    )).Where(p=>p.IsBefore == false)
                .ToList();

                return new BaseResponse<(List<BookingPhotoViewModel>, List<BookingPhotoViewModel>)>
                {
                    Data = (result, result2),
                    Description = "Фотографии успешно получены",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<(List<BookingPhotoViewModel>, List<BookingPhotoViewModel>)>
                {
                    Description = $"Произошла непредвиденная ошибка: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }
    }
}
