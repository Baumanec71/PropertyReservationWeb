using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Extensions;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.RentalRequest;
using PropertyReservationWeb.Service.Interfaces;

namespace PropertyReservationWeb.Service.Implementations
{
    public class RentalRequestService : IRentalRequestService
    {
        const int pageSize = 20;
        private readonly IBaseRepository<RentalRequest> _rentalRequestRepository;
        private readonly IBaseRepository<Advertisement> _advertisementRepository;
        private readonly IBaseRepository<PaymentRentalRequest> _paymentRentalRequestRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IPaymentService _paymentService;
        
        public RentalRequestService(
            IBaseRepository<RentalRequest> rentalRequestRepository,
            IBaseRepository<User> userRepository,
            IBaseRepository<Advertisement> advertisementRepository,
            IBaseRepository<PaymentRentalRequest> paymentRentalRequestRepository,
            IPaymentService paymentService
            ) 
        {
            _paymentService = paymentService;
            _rentalRequestRepository = rentalRequestRepository;
            _paymentRentalRequestRepository = paymentRentalRequestRepository;
            _userRepository = userRepository;
            _advertisementRepository = advertisementRepository;
        }

        private decimal GetPrice(DateTime bookingStartDate, DateTime bookingFinishDate, decimal rentalPrice)
        {
            // Рассчитываем количество дней аренды
            int rentalDays = ((bookingFinishDate - bookingStartDate).Days)+1;
            // Рассчитываем итоговую стоимость
            decimal totalPrice = rentalDays * rentalPrice;

            return totalPrice;
        }

        public async Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTrueAdvertisementForUser(long id, long idUser)
        {
            try
            {
                var rentalrequest = await _rentalRequestRepository
                    .GetAll()
                    .Include(rr => rr.Advertisement)
                    .FirstOrDefaultAsync(rr => rr.Id == id);

                if (rentalrequest == null)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Данного запроса на бронирование больше не существует",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }
               
                if(rentalrequest.ApprovalStatus == ApprovalStatus.Paid || rentalrequest.ApprovalStatus == ApprovalStatus.Completed || rentalrequest.ApprovalStatus == ApprovalStatus.Approved)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Запрос на аренду уже одобрен",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }

                var rentalRequests = await _rentalRequestRepository
                    .GetAll()
                    .Where(rr => (rr.ApprovalStatus == ApprovalStatus.Completed || rr.ApprovalStatus == ApprovalStatus.Paid) &&
                        (
                            (rr.BookingStartDate < rentalrequest.BookingFinishDate && rr.BookingFinishDate > rentalrequest.BookingStartDate) ||
                            (rentalrequest.BookingStartDate < rr.BookingFinishDate && rentalrequest.BookingFinishDate > rr.BookingStartDate) ||
                            (rentalrequest.BookingStartDate <= rr.BookingStartDate && rentalrequest.BookingFinishDate >= rr.BookingFinishDate) ||
                            (rr.BookingStartDate <= rentalrequest.BookingStartDate && rr.BookingFinishDate >= rentalrequest.BookingFinishDate)
                        )
                    )
                    .ToListAsync();

                if (rentalRequests.Count != 0)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Выбранные даты уже забронированы",
                        StatusCode = StatusCode.DateBooked
                    };
                }

                if (rentalrequest.Advertisement == null || rentalrequest.Advertisement.IdAuthor != idUser)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Данного объявления не существует либо вы не являетесь его автором",
                        StatusCode = StatusCode.AdvertisementNotFound,
                    };
                }

                string paymentUrl = null;
                string paymentId = null;

                if (string.IsNullOrEmpty(rentalrequest.PaymentActiveId))
                {
                    var amount = rentalrequest.Advertisement.FixedPrepaymentAmount;
                    var description = $"Передоплата заявки на бронирование недвижимости №{rentalrequest.Id}";
                    var returnUrl = $"https://nicesait71front.serveo.net/rentalRequest/{rentalrequest.Id}";
                    var payment = await _paymentService.CreatePaymentAsync(amount, description, returnUrl);

                    if (payment != null && payment.Confirmation.ConfirmationUrl != null)
                    {
                        paymentUrl = payment.Confirmation.ConfirmationUrl;
                        paymentId = payment.Id;
                    }
                    else
                    {
                        return new BaseResponse<RentalRequestViewModel>()
                        {
                            Description = "Не удалось создать платёж",
                            StatusCode = StatusCode.PaymentError
                        };
                    }

                    var paymentRentalRequest = new PaymentRentalRequest()
                    {
                        Id = paymentId,
                        RentalRequestId = rentalrequest.Id,
                        Amount = rentalrequest.Advertisement.FixedPrepaymentAmount,
                        Status = PaymentStatusDb.Pending,
                        CreateDate = DateTime.UtcNow,
                        Url = paymentUrl
                    };

                    await _paymentRentalRequestRepository.Create(paymentRentalRequest);
                    rentalrequest.PaymentActiveId = paymentRentalRequest.Id;
                }
                else
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Что-то пошло не так",
                        StatusCode = StatusCode.InternalServerError,
                    };
                }

                rentalrequest.ApprovalStatus = ApprovalStatus.Approved;
                rentalrequest.DataChangeStatus = DateTime.UtcNow;

                await _rentalRequestRepository.Update(rentalrequest);

                var rentalView = new RentalRequestViewModel(
                    rentalrequest.Id,
                    rentalrequest.ApprovalStatus.GetDisplayName(),
                    rentalrequest.BookingStartDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalrequest.BookingFinishDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalrequest.DataChangeStatus.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalrequest.IdAuthorRentalRequest,
                    rentalrequest.IdNeedAdvertisement,
                    rentalrequest.Advertisement.IdAuthor,
                    rentalrequest.PaymentActiveId,
                    GetPrice(rentalrequest.BookingStartDate, rentalrequest.BookingFinishDate, rentalrequest.Advertisement.RentalPrice));

                return new BaseResponse<RentalRequestViewModel>()
                {
                    Data = rentalView,
                    Description = "Запрос на аренду найден",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = $"[CreateApprovalStatusTrueAdvertisementForUser]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusFalseAdvertisementForUser(long id, long idUser)
        {
            try
            {
                var rentalrequest = await _rentalRequestRepository
                    .GetAll()
                    .Include(rr => rr.Advertisement)
                    .FirstOrDefaultAsync(rr => rr.Id == id);

                if (rentalrequest == null || rentalrequest.ApprovalStatus == ApprovalStatus.Completed)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Данного запроса больше не существует или бронь уже началась и ее невозможжно отменить",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }

                if (rentalrequest.ApprovalStatus == ApprovalStatus.Rejected)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Запрос на аренду уже отклонен",
                        StatusCode = StatusCode.OK,
                    };
                }

                if (rentalrequest.Advertisement == null || rentalrequest.Advertisement.IdAuthor != idUser)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Данного объявления не существует либо вы не являетесь его автором",
                        StatusCode = StatusCode.AdvertisementNotFound,
                    };
                }

                if (!string.IsNullOrEmpty(rentalrequest.PaymentActiveId))
                {
                    var paymentActive = await _paymentRentalRequestRepository
                        .GetAll()
                        .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveId);

                    if (paymentActive != null && paymentActive.Status == PaymentStatusDb.Succeeded && (DateTime.UtcNow - rentalrequest.BookingStartDate).TotalDays >= 3)
                    {
                        var paymetDelete = await _paymentService.CreateRefundAsync(paymentActive.Id!, true);

                        if (paymetDelete.StatusCode != StatusCode.OK)
                        {
                            return new BaseResponse<RentalRequestViewModel>()
                            {
                                Description = paymetDelete.Description,
                                StatusCode = paymetDelete.StatusCode,
                            };
                        }
                    }
                    else if(paymentActive != null && paymentActive.Status == PaymentStatusDb.Succeeded)
                    {
                        var paymetDelete = await _paymentService.CreateRefundAsync(paymentActive.Id!, false);

                        if (paymetDelete.StatusCode != StatusCode.OK)
                        {
                            return new BaseResponse<RentalRequestViewModel>()
                            {
                                Description = paymetDelete.Description,
                                StatusCode = paymetDelete.StatusCode,
                            };
                        }
                    }
                }

                rentalrequest.ApprovalStatus = ApprovalStatus.Rejected;
                rentalrequest.PaymentActiveId = null;
                await _rentalRequestRepository.Update(rentalrequest);

                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = "Запрос на аренду отклонен",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = $"[CreateApprovalStatusFalseAdvertisementForUser]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<RentalRequestViewModel>> DeleteRentalRequestForUser(long id, long idUser)
        {
            try
            {
                var rentalrequest = await _rentalRequestRepository
                    .GetAll()
                    .FirstOrDefaultAsync(rr => rr.Id == id);

                if (rentalrequest == null)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Данного запроса больше не существует",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }

                if (rentalrequest.BookingStartDate <= DateTime.UtcNow)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Бронирование ужу началось, отменить невозможно",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }

                if(rentalrequest.PaymentActiveId != null)
                {
                    var paymentActive = await _paymentRentalRequestRepository
                        .GetAll()
                        .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveId);

                    if (paymentActive != null && paymentActive.Status == PaymentStatusDb.Succeeded)
                    {
                        await _paymentService.CreateRefundAsync(paymentActive.Id!, true);
                    }

                    rentalrequest.PaymentActiveId = null;
                }

                rentalrequest.DeleteStatus = true;
                rentalrequest.ApprovalStatus = ApprovalStatus.Rejected;
                rentalrequest.DataChangeStatus = DateTime.UtcNow;
                await _rentalRequestRepository.Update(rentalrequest);

                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = "Запрос на аренду удален",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = $"[DeleteAdvertisementForUser]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<RentalRequestViewModel>> DeleteRentalRequests()
        {
            try
            {
                var rentalrequest = await _rentalRequestRepository
                    .GetAll()
                    .Where(rr => rr.ApprovalStatus == ApprovalStatus.Rejected && (DateTime.UtcNow - rr.DataChangeStatus).TotalDays >= 3)
                    .ExecuteUpdateAsync(s => s.SetProperty(rr=> rr.DeleteStatus, true));

                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = "Запрос на аренду удален",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = $"[DeleteRentalRequests]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<RentalRequestViewModel>> CreateRentalRequest(CreateRentalRequestViewModel model, long IdAuthorRentalRequest)
        {
            try
            {

                if (model.BookingFinishDate == null || model.BookingStartDate == null)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Выберите даты бронирования",
                        StatusCode = StatusCode.DateBooked
                    };
                }
                model.BookingFinishDate = ((DateTime)model.BookingFinishDate).ToUniversalTime();
                model.BookingStartDate = ((DateTime)model.BookingStartDate).ToUniversalTime();

                var rentalRequests = await _rentalRequestRepository
                    .GetAll()
                    .Where(rr => (rr.ApprovalStatus == ApprovalStatus.Completed || rr.ApprovalStatus == ApprovalStatus.Paid && rr.DeleteStatus != true) &&
                        (
                            (rr.BookingStartDate < model.BookingFinishDate && rr.BookingFinishDate > model.BookingStartDate) ||
                            (model.BookingStartDate < rr.BookingFinishDate && model.BookingFinishDate > rr.BookingStartDate) ||
                            (model.BookingStartDate <= rr.BookingStartDate && model.BookingFinishDate >= rr.BookingFinishDate) ||
                            (rr.BookingStartDate <= model.BookingStartDate && rr.BookingFinishDate >= model.BookingFinishDate)
                        )
                    )
                    .ToListAsync();

                if (rentalRequests.Count != 0)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Выбранные даты уже забронированы",
                        StatusCode = StatusCode.DateBooked
                    };
                }

                var createRentalRequest = new RentalRequest()
                {
                    BookingStartDate = (DateTime)model.BookingStartDate!,
                    BookingFinishDate = (DateTime)model.BookingFinishDate!,
                    ApprovalStatus = ApprovalStatus.UnderСonsideration,
                    IdAuthorRentalRequest = IdAuthorRentalRequest,
                    IdNeedAdvertisement = model.IdNeedAdvertisement,
                    DeleteStatus = false,  
                };

                await _rentalRequestRepository.Create(createRentalRequest);

                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = "Запрос на аренду отправлен",
                    StatusCode = StatusCode.OK,
                };
            }
            catch(Exception ex)
            {
                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = $"[CreateRentalRequest]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<RentalRequestViewModel>> GetRentalRequest(long id)
        {
            try
            {
                var rentalrequest = await _rentalRequestRepository
                    .GetAll()
                    .Include(x=>x.Advertisement)
                    .FirstOrDefaultAsync(rr => rr.Id == id);

                if(rentalrequest == null)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Данного запроса больше не существует",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }

                var rentalView = new RentalRequestViewModel(
                    rentalrequest.Id,
                    rentalrequest.ApprovalStatus.GetDisplayName(),
                    rentalrequest.BookingStartDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalrequest.BookingFinishDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalrequest.DataChangeStatus.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalrequest.IdAuthorRentalRequest,
                    rentalrequest.IdNeedAdvertisement,
                    rentalrequest.Advertisement.IdAuthor,
                    rentalrequest.PaymentActiveId,
                    GetPrice(rentalrequest.BookingStartDate, rentalrequest.BookingFinishDate, rentalrequest.Advertisement.RentalPrice));

                return new BaseResponse<RentalRequestViewModel>()
                {
                    Data = rentalView,
                    Description = "Запрос на аренду найден",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = $"[GetRentalRequest]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<List<DateTime>>> GetAllBookedDates(long id)
        {
            try
            {
                var bookedDates = await _rentalRequestRepository
                    .GetAll()
                    .Where(rr => rr.IdNeedAdvertisement == id && rr.DeleteStatus != true &&
                                (rr.ApprovalStatus == ApprovalStatus.Completed || rr.ApprovalStatus == ApprovalStatus.Paid))
                    .Select(rr => new { rr.BookingStartDate, rr.BookingFinishDate })
                    .ToListAsync();

                var distinctDates = bookedDates
                    .SelectMany(rr => new[] { rr.BookingStartDate, rr.BookingFinishDate })
                    .Distinct()
                    .OrderBy(date => date)
                    .Select(date => date.ToLocalTime())
                    .ToList();

                return new BaseResponse<List<DateTime>>()
                {
                    Data = distinctDates,
                    Description = "Забронированные даты получены",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<DateTime>>()
                {
                    Description = $"[GetAllBookedDates]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        private async Task<IBaseResponse<(List<T>, int)>> Filter<T>(int page, RentalRequestFilterModel filterModel)
        {
            try
            {
                var query = _rentalRequestRepository
                    .GetAll()
                    .Include(x => x.Advertisement)
                    .AsQueryable();

                if (filterModel.SelectedApprovalStatus.HasValue)
                    query = query.Where(x => x.ApprovalStatus == filterModel.SelectedApprovalStatus.Value);

                if (filterModel.SelectedDeleteStatus.HasValue)
                    query = query.Where(x => x.DeleteStatus == filterModel.SelectedDeleteStatus.Value);

                if (filterModel.SelectedBookingStartDate.HasValue)
                    query = query.Where(x => x.BookingStartDate >= filterModel.SelectedBookingStartDate.Value);

                if (filterModel.SelectedBookingFinishDate.HasValue)
                    query = query.Where(x => x.BookingFinishDate <= filterModel.SelectedBookingFinishDate.Value);

                if (filterModel.SelectedIdAuthorRentalRequest.HasValue)
                    query = query.Where(x => x.IdAuthorRentalRequest == filterModel.SelectedIdAuthorRentalRequest.Value);

                if (filterModel.SelectedIdNeedAdvertisement.HasValue)
                    query = query.Where(x => x.IdNeedAdvertisement == filterModel.SelectedIdNeedAdvertisement.Value);

                if (filterModel.SelectedIdAuthorNeedAdvertisement.HasValue)
                    query = query.Where(x => x.Advertisement.IdAuthor == filterModel.SelectedIdAuthorNeedAdvertisement.Value);

                if (!string.IsNullOrEmpty(filterModel.SelectedPaymentId))
                    query = query.Where(x => x.PaymentActiveId == filterModel.SelectedPaymentId);

                var skip = (page - 1) * pageSize;
                var totalCount = await query.CountAsync();

                if (totalCount == 0)
                {
                    return new BaseResponse<(List<T>, int)>
                    {
                        Description = "Объявлений по заданным критериям нет",
                        StatusCode = StatusCode.AdvertisementNotFound
                    };
                }


                var rawData = await query
                    .Skip(skip)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                var result = rawData.Select(x => new RentalRequestViewModel
                (
                    x.Id,
                    x.ApprovalStatus.GetDisplayName(),
                    x.BookingStartDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    x.BookingFinishDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    x.DataChangeStatus.ToLocalTime().ToString("dd-MM-yyyy"),
                    x.IdAuthorRentalRequest,
                    x.IdNeedAdvertisement,
                    x.Advertisement.IdAuthor,
                    x.PaymentActiveId,
                    GetPrice(x.BookingStartDate, x.BookingFinishDate, x.Advertisement.RentalPrice)
                )).ToList();

                var rentalRequests = result.Cast<T>().ToList();

                return new BaseResponse<(List<T>, int)>
                {
                    Data = (rentalRequests, totalCount)!,
                    Description = "Фильтрация выполнена успешно",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<(List<T>, int)>
                {
                    Description = $"[Filter]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<PaginatedRentalViewModelResponse<RentalRequestViewModel>>> GetRentalRequests(int page, RentalRequestFilterModel filterModel)
        {
            try
            {
                var filterResult = await Filter<RentalRequestViewModel>(page, filterModel);

                if (filterResult.StatusCode != StatusCode.OK)
                {
                    return new BaseResponse<PaginatedRentalViewModelResponse<RentalRequestViewModel>>
                    {
                        Description = filterResult.Description,
                        StatusCode = filterResult.StatusCode
                    };
                }

                var response = new PaginatedRentalViewModelResponse<RentalRequestViewModel>
                (
                    filterResult.Data.Item1,
                    (int)(filterResult.Data.Item2 / pageSize),
                    filterModel
                );

                return new BaseResponse<PaginatedRentalViewModelResponse<RentalRequestViewModel>>
                {
                    Data = response,
                    StatusCode = StatusCode.OK,
                    Description = "Успешно получены объявления"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginatedRentalViewModelResponse<RentalRequestViewModel>>
                {
                    Description = $"[GetRentalRequests]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public Task<List<ApprovalStatusOptionViewModel>> GetAllApprovalStatus()
        {
            var objectTypes = Enum.GetValues(typeof(ApprovalStatus))
                .Cast<ApprovalStatus>()
                .Select(o => new ApprovalStatusOptionViewModel(o, o.GetDisplayName()))
                .ToList();

            return Task.FromResult(objectTypes);
        }
    }
}
