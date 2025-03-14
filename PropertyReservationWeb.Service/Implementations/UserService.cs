using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Extensions;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.User;
using PropertyReservationWeb.Service.Interfaces;

namespace PropertyReservationWeb.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<RentalRequest> _rentalRequestRepository;
        private readonly IBaseRepository<Advertisement> _advertisementRepository;
        private readonly IBaseRepository<Review> _reviewRepository;

        public UserService(IBaseRepository<User> userRepository, IBaseRepository<RentalRequest> rentalRequestRepository, IBaseRepository<Advertisement> advertisementRepository, IBaseRepository<Review> reviewRepository)
        {
            _userRepository = userRepository;
            _rentalRequestRepository = rentalRequestRepository;
            _advertisementRepository = advertisementRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<IBaseResponse<UserViewModel>> GetUserEmail(string email)
        {
            var user = await _userRepository
                .GetAll()
                .FirstOrDefaultAsync(x=>x.Email==email);

            if (user == null)
            {
                return new BaseResponse<UserViewModel>()
                {
                    Description = "Пользователь не найден",
                    Data = null,
                    StatusCode = StatusCode.UserNotFound,
                };
            }

            var countrentalRequests = await _rentalRequestRepository
                .GetAll()
                .Where(x => x.ApprovalStatus == ApprovalStatus.EndApproved &&
                       (x.IdAuthorRentalRequest == user.Id || x.Advertisement.IdAuthor == user.Id))
                .CountAsync();

            var userViewModel = new UserViewModel(
                user.Id,
                user.Password,
                user.Email,
                user.Role.GetDisplayName(),
                user.Name,
                user.Status,
                user.Rating,
                user.PhoneNumber,
                user.DateOfRegistration.ToString("yyyy-MM-dd HH:mm:ss"),
                user.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(user.Avatar)}" : null,
                user.Advertisements?.Count() ?? 0,
                countrentalRequests
            );

            return new BaseResponse<UserViewModel>()
            {
                Data = userViewModel,
                Description = "Пользователь наден",
                StatusCode = StatusCode.OK,
            };
        }

        public async Task<IBaseResponse<UserViewModel>> GetUserId(long id)
        {
            var taskuser = _userRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == id);

            var user = await taskuser;

            if (user == null)
            {
                return new BaseResponse<UserViewModel>()
                {
                    Description = "Пользователь не найден",
                    Data = null,
                    StatusCode = StatusCode.UserNotFound,
                };
            }

            var countrentalRequests = await _rentalRequestRepository
                .GetAll()
                .Where(x => x.ApprovalStatus == ApprovalStatus.EndApproved &&
                (x.IdAuthorRentalRequest == user.Id || x.Advertisement.IdAuthor == user.Id))
                .CountAsync();

            var userViewModel = new UserViewModel(
                user.Id,
                user.Password,
                user.Email,
                user.Role.GetDisplayName(),
                user.Name,
                user.Status,
                user.Rating,
                user.PhoneNumber,
                user.DateOfRegistration.ToString("yyyy-MM-dd HH:mm:ss"),
                user.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(user.Avatar)}" : null,
                user.Advertisements?.Count() ?? 0,
                countrentalRequests
            );

            return new BaseResponse<UserViewModel>()
            {
                Data = userViewModel,
                Description = "Пользователь найден",
                StatusCode = StatusCode.OK,
            };
        }

        public async Task<IBaseResponse<PaginatedViewModelResponse<UserViewModel>>> GetUsers(int page)
        {
            try
            {
                const int pageSize = 20;

                if (page < 1 || pageSize < 1)
                {
                    return new BaseResponse<PaginatedViewModelResponse<UserViewModel>>()
                    {
                        Description = "Некорректные параметры страницы",
                        StatusCode = StatusCode.InvalidParameters
                    };
                }

                var totalUsers = await _userRepository
                    .GetAll()
                    .CountAsync();

                if (totalUsers == 0)
                {
                    return new BaseResponse<PaginatedViewModelResponse<UserViewModel>>()
                    {
                        Description = "Пользователи не найдены",
                        StatusCode = StatusCode.UserNotFound
                    };
                }

                var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
                var users = await _userRepository
                    .GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                var userIds = users
                    .Select(u => u.Id);

                var rentalCounts = await _rentalRequestRepository
                    .GetAll()
                    .Where(x => x.ApprovalStatus == ApprovalStatus.EndApproved &&
                               (userIds.Contains(x.IdAuthorRentalRequest) || userIds.Contains(x.Advertisement.IdAuthor)))
                    .GroupBy(x => x.IdAuthorRentalRequest)
                    .Select(g => new { UserId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.UserId, x => x.Count);

                var usersView = users.Select(user => new UserViewModel(
                    user.Id,
                    user.Password,
                    user.Email,
                    user.Role.GetDisplayName(),
                    user.Name,
                    user.Status,
                    user.Rating,
                    user.PhoneNumber,
                    user.DateOfRegistration.ToString("yyyy-MM-dd HH:mm:ss"),
                    user.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(user.Avatar)}" : null,
                    user.Advertisements?.Count() ?? 0,
                    rentalCounts.TryGetValue(user.Id, out int count) ? count : 0
                )).ToList();

                var response = new PaginatedViewModelResponse<UserViewModel>
                (
                    usersView,
                    totalPages,
                    null
                );

                return new BaseResponse<PaginatedViewModelResponse<UserViewModel>>()
                {
                    Data = response,
                    Description = "Пользователи найдены",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginatedViewModelResponse<UserViewModel>>()
                {
                    Description = $"[GetUsers]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<UserViewModel>> DeleteUser(long id)
        {
            try
            {
                var user = await _userRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return new BaseResponse<UserViewModel>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound
                    };

                }

                user.Status = true;
                await _userRepository.Update(user);

                var countrentalRequests = await _rentalRequestRepository
                    .GetAll()
                    .Where(x => x.ApprovalStatus == ApprovalStatus.EndApproved &&
                    (x.IdAuthorRentalRequest == user.Id || x.Advertisement.IdAuthor == user.Id))
                    .CountAsync();

                var userforuser = new UserViewModel(
                user.Id,
                user.Password,
                user.Email,
                user.Role.GetDisplayName(),
                user.Name,
                user.Status,
                user.Rating,
                user.PhoneNumber,
                user.DateOfRegistration.ToString("yyyy-MM-dd HH:mm:ss"),
                user.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(user.Avatar)}" : null,
                user.Advertisements?.Count() ?? 0,
                countrentalRequests
                );

                return new BaseResponse<UserViewModel>()
                {
                    Data = userforuser,
                    Description = "Пользователь удален",
                    StatusCode = StatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<UserViewModel>()
                {
                    Description = $"[DeleteUser]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<UserViewModel>> Update(UpdateUserViewModel model, long id)
        {
            try
            {
                if(!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    var user0 = await _userRepository
                        .GetAll()
                        .FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber && x.Id != id);

                    if (user0 != null)
                    {
                        return new BaseResponse<UserViewModel>()
                        {
                            Description = "Данный номер телефона занят",
                            StatusCode = StatusCode.UserAlreadyExists
                        };
                    }
                }

                var user = await _userRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return new BaseResponse<UserViewModel>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound
                    };
                }

                if (!string.IsNullOrEmpty(model.Avatar))
                {
                    try
                    {
                        byte[] avatarBytes = Convert.FromBase64String(model.Avatar);
                        user.Avatar = avatarBytes;
                    }
                    catch (FormatException)
                    {
                        return new BaseResponse<UserViewModel>()
                        {
                            Description = "Ошибка файла",
                            StatusCode = StatusCode.UserNotFound
                        };
                    }
                }
                else
                {
                    user.Avatar = null;
                }

                user.Name = model.Name ?? "";
                user.PhoneNumber = model.PhoneNumber ?? "";
                
                await _userRepository.Update(user);

                var countrentalRequests = await _rentalRequestRepository
                    .GetAll()
                    .Where(x => x.ApprovalStatus == ApprovalStatus.EndApproved &&
                    (x.IdAuthorRentalRequest == user.Id || x.Advertisement.IdAuthor == user.Id))
                    .CountAsync();

                var userforuser = new UserViewModel(
                user.Id,
                user.Password,
                user.Email,
                user.Role.GetDisplayName(),
                user.Name,
                user.Status,
                user.Rating,
                user.PhoneNumber,
                user.DateOfRegistration.ToString("yyyy-MM-dd HH:mm:ss"),
                user.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(user.Avatar)}" : null,
                user.Advertisements?.Count() ?? 0,
                countrentalRequests
                );

                return new BaseResponse<UserViewModel>()
                {
                    Data = userforuser,
                    Description = "Данные обновлены",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {    
                return new BaseResponse<UserViewModel>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Внутренняя ошибка: {ex.Message}"
                };
            }
        }

        public async Task<IBaseResponse<User>> CalculatingTheRatingUser(long id)
        {
            try
            {
                var user = await _userRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return new BaseResponse<User>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound
                    };

                }

                var listIdRentalRequests = await _advertisementRepository
                    .GetAll()
                    .Where(ad => ad.IdAuthor == id)
                    .Include(ad => ad.RentalRequests)
                    .SelectMany(ad => ad.RentalRequests
                        .Where(r => (r.IdNeedAdvertisement == ad.Id || r.IdAuthorRentalRequest == id)
                                     && r.ApprovalStatus == ApprovalStatus.Approved)
                        .Select(r => new { Advertisement = ad, RentalRequestId = r.Id }))
                    .ToListAsync();

                var reviews = await _reviewRepository
                    .GetAll()
                    .Where(x => listIdRentalRequests
                        .Select(tuple => tuple.RentalRequestId)
                        .Contains(x.IdNeedRentalRequest) && x.StatusDel != true && (
                        (x.IsTheLandlord == false && listIdRentalRequests.Any(tuple => tuple.Advertisement.Id == x.IdNeedRentalRequest))
                        || (x.IsTheLandlord == true && listIdRentalRequests.Any(tuple => tuple.RentalRequestId == x.IdNeedRentalRequest))
                        ))
                    .ToListAsync();

                user.Rating = reviews.Any()
                    ? Math.Round(reviews.Average(r => r.TheQualityOfTheTransaction), 2)
                    : 0;

                await _userRepository.Update(user);

                return new BaseResponse<User>()
                {

                    Data = user,
                    Description = "Рейтинг пользователя подсчитан",
                    StatusCode = StatusCode.OK

                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<User>()
                {
                    Description = $"[CalculatingTheRatingUser]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public BaseResponse<Dictionary<int, string>> GetRoles()
        {
            try
            {
                var roles = ((Role[])Enum.GetValues(typeof(Role)))
                    .ToDictionary(k => (int)k, t => t.GetDisplayName());

                return new BaseResponse<Dictionary<int, string>>()
                {
                    Data = roles,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Dictionary<int, string>>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
