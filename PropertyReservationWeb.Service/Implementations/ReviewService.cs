using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.DAL.Repositories;
using PropertyReservationWeb.Domain.Helpers;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.RentalRequest;
using PropertyReservationWeb.Domain.ViewModels.Review;
using PropertyReservationWeb.Service.Interfaces;

namespace PropertyReservationWeb.Service.Implementations
{
    public class ReviewService : IReviewService
    {

        private readonly IBaseRepository<Review> _reviewRepository;
        private readonly IBaseRepository<Advertisement> _advertisementRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IUserService _userService;
        private readonly IAdvertisementService _advertisementService;
        private readonly IBaseRepository<RentalRequest> _rentalRepository;
        private readonly IBaseRepository<BonusTransaction> _bonusTransactionRepository;
        private readonly IOptions<BonusSettings> _bonusSettings;

        public ReviewService(IBaseRepository<Review> reviewRepository, IUserService userService, IAdvertisementService advertisementService, IBaseRepository<User> userRepository, IOptions<BonusSettings> bonusSettings, IBaseRepository<RentalRequest> rentalRepository, IBaseRepository<BonusTransaction> bonusTransactionRepository, IBaseRepository<Advertisement> advertisementRepository)
        {
            _userService = userService;
            _advertisementService = advertisementService;
            _userRepository = userRepository;
            _reviewRepository = reviewRepository;
            _rentalRepository = rentalRepository;
            _bonusTransactionRepository = bonusTransactionRepository;
            _advertisementRepository = advertisementRepository;
            _bonusSettings = bonusSettings;
        }

        private decimal CalculateBonusAmount(RentalRequest rental, Review review)
        {
            var settings = _bonusSettings.Value;

            int rentalDays = (rental.BookingFinishDate - rental.BookingStartDate).Days;
            rentalDays = Math.Max(rentalDays, 1);

            decimal basePerDay = review.IsTheLandlord
                ? settings.LandlordBonusPerDay
                : settings.RenterBonusPerDay;

            settings.RatingMultipliers.TryGetValue(review.TheQualityOfTheTransaction, out var multiplier);
            multiplier = multiplier == 0 ? 1.0m : multiplier;

            return Math.Round(rentalDays * basePerDay * multiplier, 2);
        }

        public async Task<IBaseResponse<ReviewViewModel>> CreateReview(CreateReviewViewModel model, long IdUser)
        {
            try
            {
                var rental = await _rentalRepository
                    .GetAll()
                    .Include(rr => rr.User)
                    .Include(rr =>rr.Advertisement)
                    .ThenInclude(a=>a.User)
                    .Include(rr => rr.Reviews)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rr => rr.Id == model.IdRental);

                if (rental == null || rental.DeleteStatus == true || rental.ApprovalStatus != Domain.Enum.ApprovalStatus.Completed)
                {
                    return new BaseResponse<ReviewViewModel>
                    {
                        StatusCode = Domain.Enum.StatusCode.RentalRequestNotFound,
                        Description = "Запрос на аренду не найден"
                    };
                }

                long IdUserBal;
                bool isAuthorRental = rental.User.Id == IdUser;
                bool isAuthorAdvertisement = rental.Advertisement.User.Id == IdUser;

                if (!isAuthorRental && !isAuthorAdvertisement)
                {
                    return new BaseResponse<ReviewViewModel>
                    {
                        StatusCode = Domain.Enum.StatusCode.CreateReviewError,
                        Description = "Вы не можете создать отзыв"
                    };
                }

                var review = new Review(model.TheQualityOfTheTransaction, model.Comment, false, DateTime.UtcNow, model.IdRental, true, false);

                if (isAuthorRental)
                {
                    var hasReview = await _rentalRepository
                        .GetAll()
                        .AsNoTracking()
                        .AnyAsync(rr =>
                            rr.IdAuthorRentalRequest == IdUser &&
                            rr.IdNeedAdvertisement == rental.Advertisement.Id &&
                            rr.Reviews.Any(r => r.IsTheLandlord == false && r.StatusDel == false));

                    if (hasReview)
                    {
                        return new BaseResponse<ReviewViewModel>
                        {
                            StatusCode = Domain.Enum.StatusCode.CreateReviewError,
                            Description = "Вы уже оставили отзыв"
                        };
                    }

                    review.IsTheLandlord = false;
                    IdUserBal = rental.Advertisement.User.Id;

                    await _reviewRepository.Create(review);
                    await _advertisementService.CalculatingTheRating(rental.Advertisement.Id);
                    await _userService.CalculatingTheRatingUser(IdUserBal);
                }
                else if(isAuthorAdvertisement)
                {
                    var hasReview = await _reviewRepository.GetAll()
                        .AsNoTracking()
                        .AnyAsync(r =>
                            r.IsTheLandlord == true &&
                            r.StatusDel == false &&
                            r.RentalRequest.Advertisement.User.Id == IdUser &&
                            r.RentalRequest.User.Id == rental.User.Id);

                    if (hasReview)
                    {
                        return new BaseResponse<ReviewViewModel>
                        {
                            StatusCode = Domain.Enum.StatusCode.CreateReviewError,
                            Description = "Вы уже оставили отзыв на данного жильца"
                        };
                    }

                    IdUserBal = rental.User.Id;

                    await _reviewRepository.Create(review);
                    await _userService.CalculatingTheRatingUser(IdUserBal);
                }
                else
                {
                    return new BaseResponse<ReviewViewModel>
                    {
                        StatusCode = Domain.Enum.StatusCode.CreateReviewError,
                        Description = "Вы не можете создать отзыв"
                    };
                }

                var bonusTransaction = new BonusTransaction()
                {
                    Amount = CalculateBonusAmount(rental, review),
                    IsCalculated = false,
                    ReviewId = review.Id,
                    Type = "Начисление",
                    DateCreate = DateTime.UtcNow,
                    UserId = IdUserBal,
                    Description = "Начисление за отзыв",
                };

                await _bonusTransactionRepository.Create(bonusTransaction);

                await _userService.CalculatingTheBonusPointsUser(IdUserBal);

                return new BaseResponse<ReviewViewModel>
                {
                    //Data = new ReviewViewModel(
                    //    review.Id,
                    //    review.TheQualityOfTheTransaction,
                    //    review.Comment,
                    //    review.DateOfCreation.ToString(),
                    //    review.IdNeedRentalRequest,
                    //    IdUser,
                    //    IdUserBal
                    //),
                    StatusCode = Domain.Enum.StatusCode.OK,
                    Description = "Отзыв создан успешно"
                };

            }
            catch(Exception ex)
            {
                return new BaseResponse<ReviewViewModel>
                {
                    StatusCode = Domain.Enum.StatusCode.InternalServerError,
                    Description = $"Ошибка в создании отзыва {ex}"
                };
            }
        }

        public async Task<IBaseResponse<ReviewViewModel>> DeleteReview(long id)
        {
            try
            {
                var review = await _reviewRepository
                    .GetAll()
                    .Include(r => r.RentalRequest)
                        .ThenInclude(rr => rr.Advertisement)
                            .ThenInclude(a => a.User)
                    .Include(r => r.RentalRequest)
                        .ThenInclude(rr => rr.User)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (review == null || review.StatusDel)
                {
                    return new BaseResponse<ReviewViewModel>
                    {
                        StatusCode = Domain.Enum.StatusCode.ReviewNotFound,
                        Description = "Отзыв не найден или уже удалён"
                    };
                }

                review.StatusDel = true;
                await _reviewRepository.Update(review);

                var originalBonus = await _bonusTransactionRepository
                    .GetAll()
                    .FirstOrDefaultAsync(bt => bt.ReviewId == review.Id);

                if (originalBonus != null)
                {
                    var negativeTransaction = new BonusTransaction
                    {
                        Amount = -originalBonus.Amount,
                        IsCalculated = false,
                        ReviewId = review.Id,
                        Type = "Списание",
                        DateCreate = DateTime.UtcNow,
                        UserId = originalBonus.UserId,
                        Description = "Списание бонусов за удалённый отзыв"
                    };

                    await _bonusTransactionRepository.Create(negativeTransaction);
                }

                if (review.IsTheLandlord == true)
                {
                     await _userService.CalculatingTheRatingUser(review.RentalRequest.User.Id);
                     await _userService.CalculatingTheBonusPointsUser(review.RentalRequest.User.Id);
                }
                else
                {
                    await _userService.CalculatingTheRatingUser(review.RentalRequest.Advertisement.User.Id);
                    await _userService.CalculatingTheBonusPointsUser(review.RentalRequest.Advertisement.User.Id);
                    await _advertisementService.CalculatingTheRating(review.RentalRequest.Advertisement.Id);
                }  

                return new BaseResponse<ReviewViewModel>
                {
                    StatusCode = Domain.Enum.StatusCode.OK,
                    Description = "Отзыв удалён. Бонусы списаны."
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ReviewViewModel>
                {
                    StatusCode = Domain.Enum.StatusCode.InternalServerError,
                    Description = $"Ошибка при удалении отзыва: {ex.Message}"
                };
            }
        }

        public async Task<IBaseResponse<PaginatedViewModelResponse<ReviewViewModel, ReviewFilterModel>>> GetReviews(int page, ReviewFilterModel filterModel)
        {
            try
            {
                const int pageSize = 10;

                var query = _reviewRepository
                    .GetAll()
                    .AsNoTracking()
                    .Include(r => r.RentalRequest)
                        .ThenInclude(rq => rq.User)
                    .Include(r => r.RentalRequest.Advertisement)
                        .ThenInclude(ad => ad.User)
                    .Include(r => r.BonusTransactions)
                    .AsQueryable();

                query = query
                    .OrderByDescending(x => x.DateOfCreation);

                if (filterModel.SelectedDeleteStatus.HasValue)
                {
                    query = query.Where(r => r.StatusDel == filterModel.SelectedDeleteStatus.Value);
                }

                if (filterModel.IsAuthor.HasValue)
                {
                    query = query.Where(r=>r.IsTheLandlord == filterModel.IsAuthor.Value);
                }

                if (filterModel.SelectedIdAuthor.HasValue)
                {
                    query = query.Where(r => (r.RentalRequest.Advertisement.User.Id == filterModel.SelectedIdAuthor && r.IsTheLandlord == true) || 
                    (r.RentalRequest.User.Id == filterModel.SelectedIdAuthor && r.IsTheLandlord == false));
                }
                if (filterModel.SelectedIdUserWhoReview.HasValue)
                {
                    query = query.Where(r => (r.RentalRequest.Advertisement.User.Id == filterModel.SelectedIdUserWhoReview && r.IsTheLandlord == false) || 
                    (r.RentalRequest.User.Id == filterModel.SelectedIdUserWhoReview && r.IsTheLandlord == true));
                }

                if (filterModel.SelectedIdAdvertisement.HasValue)
                {
                    query = query.Where(r => r.RentalRequest.Advertisement.Id == filterModel.SelectedIdAdvertisement.Value);
                }

                if (filterModel.SelectedTheQualityOfTheTransaction.HasValue)
                {
                    query = query.Where(r => r.TheQualityOfTheTransaction == filterModel.SelectedTheQualityOfTheTransaction.Value);
                }

                if (filterModel.SelectedDate.HasValue)
                {
                    var selectedDate = filterModel.SelectedDate.Value.Date;
                    query = query.Where(r => r.DateOfCreation.Date >= selectedDate);
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var reviews = await query
                    .OrderByDescending(r => r.DateOfCreation)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                var reviewViewModels = reviews.Select(r =>
                {
                    return new ReviewViewModel(
                        r.Id,
                        r.TheQualityOfTheTransaction,
                        r.Comment,
                        r.DateOfCreation.ToString("yyyy-MM-dd HH:mm"),
                        r.IdNeedRentalRequest,
                        r.RentalRequest.User.Id,
                        r.RentalRequest.User.Name != null ? r.RentalRequest.User.Name : null,
                        r.RentalRequest.User.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(r.RentalRequest.User.Avatar)}" : null,
                        r.RentalRequest.Advertisement.User.Id
                    );
                }).ToList();

                return new BaseResponse<PaginatedViewModelResponse<ReviewViewModel, ReviewFilterModel>>
                {
                    Data = new PaginatedViewModelResponse<ReviewViewModel, ReviewFilterModel>(
                        reviewViewModels,
                        totalPages,
                        filterModel
                    ),
                    StatusCode = Domain.Enum.StatusCode.OK,
                    Description = "Отзывы получены успешно"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginatedViewModelResponse<ReviewViewModel, ReviewFilterModel>>
                {
                    StatusCode = Domain.Enum.StatusCode.InternalServerError,
                    Description = $"Ошибка при получении отзывов: {ex.Message}"
                };
            }
        }
    }
}
