using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
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
        private readonly IBaseRepository<PaymentRentalRequest> _paymentRentalRequestRepository;
        private readonly IBaseRepository<Conflict> _conflictRepositoryRepository;   
        private readonly IPaymentService _paymentService;
        private readonly string _connectionString;

        public RentalRequestService(
            IBaseRepository<RentalRequest> rentalRequestRepository,
            IBaseRepository<PaymentRentalRequest> paymentRentalRequestRepository,
            IPaymentService paymentService, IConfiguration configuration, IBaseRepository<Conflict> conflictRepositoryRepository
            ) 
        {
            _conflictRepositoryRepository = conflictRepositoryRepository;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _paymentService = paymentService;
            _rentalRequestRepository = rentalRequestRepository;
            _paymentRentalRequestRepository = paymentRentalRequestRepository;
        }

        private decimal GetPrice(DateTime bookingStartDate, DateTime bookingFinishDate, decimal rentalPrice)
        {
            int rentalDays = ((bookingFinishDate - bookingStartDate).Days)+1;
            decimal totalPrice = rentalDays * rentalPrice;

            return totalPrice;
        }

        public async Task<IBaseResponse<List<RentalRequestViewModel>>> UpdateRentalStatusComplete()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using var transaction = await connection.BeginTransactionAsync();

                    var updateRentalRequestsSql = @"
        UPDATE ""RentalRequests""
        SET ""ApprovalStatus"" = @completedStatus,
            ""DataChangeStatus"" = @now
        WHERE ""ApprovalStatus"" = @paidStatus
          AND ""BookingFinishDate"" <= @now;
    ";

                    await connection.ExecuteAsync(updateRentalRequestsSql, new
                    {
                        completedStatus = (int)ApprovalStatus.Completed,
                        paidStatus = (int)ApprovalStatus.Paid,
                        now = DateTime.UtcNow
                    }, transaction);

                    var updateUserBalancesSql = @"
        UPDATE ""Users"" u
        SET ""Balance"" = u.""Balance"" + COALESCE(sub.total, 0)
        FROM (
            SELECT a.""IdAuthor"" AS userId,
                   SUM(rr.""FixedPrepaymentAmount"") AS total
            FROM ""Advertisements"" a
            JOIN ""RentalRequests"" rr ON rr.""IdNeedAdvertisement"" = a.""Id""
            WHERE rr.""ApprovalStatus"" = @completedStatus
              AND rr.""IsCalculated"" = false
            GROUP BY a.""IdAuthor""
        ) AS sub
        WHERE u.""Id"" = sub.userId;
    ";

                    await connection.ExecuteAsync(updateUserBalancesSql, new
                    {
                        completedStatus = (int)ApprovalStatus.Completed
                    }, transaction);

                    var markRequestsCalculatedSql = @"
        UPDATE ""RentalRequests""
        SET ""IsCalculated"" = true
        WHERE ""ApprovalStatus"" = @completedStatus
          AND ""IsCalculated"" = false;
    ";

                    await connection.ExecuteAsync(markRequestsCalculatedSql, new
                    {
                        completedStatus = (int)ApprovalStatus.Completed
                    }, transaction);

                    await transaction.CommitAsync();
                }

                //// Обновляем статусы завершенных аренд
                //await _rentalRequestRepository.GetAll()
                //    .Where(rr => rr.ApprovalStatus == ApprovalStatus.Paid &&
                //                rr.BookingFinishDate <= DateTime.UtcNow)
                //    .ExecuteUpdateAsync(setters => setters
                //        .SetProperty(rr => rr.ApprovalStatus, ApprovalStatus.Completed)
                //        .SetProperty(rr => rr.DataChangeStatus, DateTime.UtcNow));

                //await _userRepository.GetAll()
                //    .Where(u => u.Advertisements.Any(ad =>
                //        ad.RentalRequests.Any(rr =>
                //            rr.ApprovalStatus == ApprovalStatus.Completed &&
                //            !rr.IsCalculated)))
                //    .ExecuteUpdateAsync(setters => setters
                //        .SetProperty(u => u.Balance,
                //            u => u.Balance + u.Advertisements
                //                .SelectMany(ad => ad.RentalRequests
                //                    .Where(rr => rr.ApprovalStatus == ApprovalStatus.Completed &&
                //                                !rr.IsCalculated))
                //                .Sum(rr => rr.FixedPrepaymentAmount)));

                //await _rentalRequestRepository.GetAll()
                //    .Where(rr => rr.ApprovalStatus == ApprovalStatus.Completed &&
                //                !rr.IsCalculated)
                //    .ExecuteUpdateAsync(setters => setters
                //        .SetProperty(rr => rr.IsCalculated, true));

                return new BaseResponse<List<RentalRequestViewModel>>()
                {
                    Description = $"Данные о объявлениях и балансах успешно обновились",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<RentalRequestViewModel>>()
                {
                    Description = $"[UpdateRentalStatusComplete]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTrueAdvertisementForUser(long id, long idUser, decimal FixedPrepaymentAmount, decimal FixedDepositAmount, bool IsPhotoSkippedByLandlord)
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

                rentalrequest.FixedPrepaymentAmount = FixedPrepaymentAmount;
                rentalrequest.FixedDepositAmount = FixedDepositAmount;
                rentalrequest.IsPhotoSkippedByLandlord = IsPhotoSkippedByLandlord;

                if (rentalrequest.ApprovalStatus == ApprovalStatus.Paid || rentalrequest.ApprovalStatus == ApprovalStatus.PaidButNewDate || rentalrequest.ApprovalStatus == ApprovalStatus.Completed || rentalrequest.ApprovalStatus == ApprovalStatus.Approved || rentalrequest.ApprovalStatus == ApprovalStatus.PaidPayment || rentalrequest.ApprovalStatus == ApprovalStatus.PaidDeposit)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Запрос на аренду уже одобрен",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }

                var rentalRequests = await _rentalRequestRepository
                    .GetAll()
                    .Where(rr => (rr.ApprovalStatus == ApprovalStatus.Completed || rr.ApprovalStatus == ApprovalStatus.Paid || rentalrequest.ApprovalStatus == ApprovalStatus.PaidPayment || rentalrequest.ApprovalStatus == ApprovalStatus.PaidDeposit) &&
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

                string paymentUrl = null!;
                string paymentId = null!;

                if (string.IsNullOrEmpty(rentalrequest.PaymentActiveDepositId) && rentalrequest.FixedDepositAmount != 0 && rentalrequest.IsPhotoSkippedByLandlord == false)
                {
                    var amount = rentalrequest.FixedDepositAmount;
                    var description = $"Внесение депозита для бронирование недвижимости №{rentalrequest.Id}";
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
                        Amount = rentalrequest.FixedPrepaymentAmount,
                        Status = PaymentStatusDb.Pending,
                        IsPayment = false,
                        CreateDate = DateTime.UtcNow,
                        Url = paymentUrl
                    };

                    await _paymentRentalRequestRepository.Create(paymentRentalRequest);

                    rentalrequest.PaymentActiveDepositId = paymentRentalRequest.Id;
                }

                if (string.IsNullOrEmpty(rentalrequest.PaymentActiveId))
                {
                    var amount = rentalrequest.FixedPrepaymentAmount;
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
                        Amount = rentalrequest.FixedPrepaymentAmount,
                        Status = PaymentStatusDb.Pending,
                        IsPayment = true,
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
                        Description = "Не удалось внести предоплату",
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
                    rentalrequest.PaymentActiveDepositId,
                    GetPrice(rentalrequest.BookingStartDate, rentalrequest.BookingFinishDate, rentalrequest.Advertisement.RentalPrice),
                    rentalrequest.FixedPrepaymentAmount,
                    rentalrequest.FixedDepositAmount,
                    rentalrequest.CheckInTime.ToString(@"hh\:mm"),
                    rentalrequest.CheckOutTime.ToString(@"hh\:mm"),
                    rentalrequest.IsBeforePhotosUploaded,
                    rentalrequest.IsAfterPhotosUploaded,
                    rentalrequest.IsPhotoSkippedByLandlord);

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

                if (rentalrequest == null)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Данного запроса больше не существует или бронь уже началась и ее невозможно отменить",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }

                if (rentalrequest == null || rentalrequest.ApprovalStatus == ApprovalStatus.Completed || rentalrequest.ApprovalStatus == ApprovalStatus.TheBookingHasStarted)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Данного запроса больше не существует или бронь уже началась и ее невозможно отменить",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }

                if (rentalrequest.ApprovalStatus == ApprovalStatus.TheUsersIsUnhappy)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Конфликт в процессе изучения пожалуйста подождите",
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

                /////////////  Разные условия по понижению в рейтинге

                if (rentalrequest.ApprovalStatus == ApprovalStatus.Paid && (DateTime.UtcNow - rentalrequest.BookingStartDate).TotalDays == 0 && ((rentalrequest.CheckInTime) - DateTime.Now.TimeOfDay).TotalHours >= 3)
                {

                }

                if (!string.IsNullOrEmpty(rentalrequest.PaymentActiveDepositId))
                {
                    var paymentActive = await _paymentRentalRequestRepository
                        .GetAll()
                        .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveDepositId);

                    if (paymentActive != null && paymentActive.Status == PaymentStatusDb.Succeeded)
                    {
                        var paymetDelete = await _paymentService.CreateRefundAsync(paymentActive.Id!, 0);

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

                if (!string.IsNullOrEmpty(rentalrequest.PaymentActiveId))
                {
                    var paymentActive = await _paymentRentalRequestRepository
                        .GetAll()
                        .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveId);

                    if(paymentActive != null && paymentActive.Status == PaymentStatusDb.Succeeded)
                    {
                        var paymetDelete = await _paymentService.CreateRefundAsync(paymentActive.Id!, 0);

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
                rentalrequest.PaymentActiveDepositId = null;
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

        public async Task<IBaseResponse<RentalRequestViewModel>> DeleteRentalRequestForUser(long id, long idUser) // отмена брони и ее удаление арендатором 
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

                if (rentalrequest.ApprovalStatus == ApprovalStatus.Completed || rentalrequest.ApprovalStatus == ApprovalStatus.TheBookingHasStarted)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Бронирование уже началось, отменить невозможно, пишите в тех поддержку мы разберемся)",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }
                else if (rentalrequest.ApprovalStatus == ApprovalStatus.TheUsersIsUnhappy)
                {
                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Кофликт рассматиривается пожалуйста подождите)",
                        StatusCode = StatusCode.RentalRequestNotFound,
                    };
                }
                else if (rentalrequest.ApprovalStatus != ApprovalStatus.TheBookingHasStarted && rentalrequest.ApprovalStatus != ApprovalStatus.TheUsersIsUnhappy && (rentalrequest.BookingStartDate - DateTime.UtcNow).TotalDays <= 7)
                {
                    if (rentalrequest.PaymentActiveDepositId != null)
                    {
                        var paymentActiveDeposit = await _paymentRentalRequestRepository
                            .GetAll()
                            .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveDepositId);

                        if (paymentActiveDeposit != null && paymentActiveDeposit.Status == PaymentStatusDb.Succeeded)
                        {
                            await _paymentService.CreateRefundAsync(paymentActiveDeposit.Id!, 0);
                        }

                        rentalrequest.PaymentActiveDepositId = null;
                    }

                    if (rentalrequest.PaymentActiveId != null)
                    {
                        var paymentActive = await _paymentRentalRequestRepository
                            .GetAll()
                            .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveId);

                        if (paymentActive != null && paymentActive.Status == PaymentStatusDb.Succeeded)
                        {
                            await _paymentService.CreateRefundAsync(paymentActive.Id!, 0.5);
                        }

                        rentalrequest.PaymentActiveId = null;
                    }

                    rentalrequest.DeleteStatus = true;
                    rentalrequest.ApprovalStatus = ApprovalStatus.Rejected;
                    rentalrequest.DataChangeStatus = DateTime.UtcNow;
                    await _rentalRequestRepository.Update(rentalrequest);

                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Запрос на аренду удален, депозит и 50 % предоплаты скоро поступят на вашу карту",
                        StatusCode = StatusCode.OK,
                    };
                }
                else if (rentalrequest.ApprovalStatus != ApprovalStatus.TheBookingHasStarted && rentalrequest.ApprovalStatus != ApprovalStatus.TheUsersIsUnhappy && (rentalrequest.BookingStartDate - DateTime.UtcNow).TotalDays <= 30)
                {
                    if (rentalrequest.PaymentActiveDepositId != null)
                    {
                        var paymentActiveDeposit = await _paymentRentalRequestRepository
                            .GetAll()
                            .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveDepositId);

                        if (paymentActiveDeposit != null && paymentActiveDeposit.Status == PaymentStatusDb.Succeeded)
                        {
                            await _paymentService.CreateRefundAsync(paymentActiveDeposit.Id!, 0);
                        }

                        rentalrequest.PaymentActiveDepositId = null;
                    }

                    if (rentalrequest.PaymentActiveId != null)
                    {
                        var paymentActive = await _paymentRentalRequestRepository
                            .GetAll()
                            .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveId);

                        if (paymentActive != null && paymentActive.Status == PaymentStatusDb.Succeeded)
                        {
                            await _paymentService.CreateRefundAsync(paymentActive.Id!, 0.7);
                        }

                        rentalrequest.PaymentActiveId = null;
                    }

                    rentalrequest.DeleteStatus = true;
                    rentalrequest.ApprovalStatus = ApprovalStatus.Rejected;
                    rentalrequest.DataChangeStatus = DateTime.UtcNow;
                    await _rentalRequestRepository.Update(rentalrequest);

                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Запрос на аренду удален, депозит и 70 % предоплаты скоро поступят на вашу карту",
                        StatusCode = StatusCode.OK,
                    };
                }
                else if (rentalrequest.ApprovalStatus != ApprovalStatus.TheBookingHasStarted && rentalrequest.ApprovalStatus != ApprovalStatus.TheUsersIsUnhappy && (rentalrequest.BookingStartDate - DateTime.UtcNow).TotalDays > 30)
                {
                    if (rentalrequest.PaymentActiveDepositId != null)
                    {
                        var paymentActiveDeposit = await _paymentRentalRequestRepository
                            .GetAll()
                            .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveDepositId);

                        if (paymentActiveDeposit != null && paymentActiveDeposit.Status == PaymentStatusDb.Succeeded)
                        {
                            await _paymentService.CreateRefundAsync(paymentActiveDeposit.Id!, 0);
                        }

                        rentalrequest.PaymentActiveDepositId = null;
                    }

                    if (rentalrequest.PaymentActiveId != null)
                    {
                        var paymentActive = await _paymentRentalRequestRepository
                            .GetAll()
                            .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveId);

                        if (paymentActive != null && paymentActive.Status == PaymentStatusDb.Succeeded)
                        {
                            await _paymentService.CreateRefundAsync(paymentActive.Id!, 0);
                        }

                        rentalrequest.PaymentActiveId = null;
                    }

                    rentalrequest.DeleteStatus = true;
                    rentalrequest.ApprovalStatus = ApprovalStatus.Rejected;
                    rentalrequest.DataChangeStatus = DateTime.UtcNow;
                    await _rentalRequestRepository.Update(rentalrequest);

                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Запрос на аренду удален, депозит и предоплата в полном размере скоро поступят на вашу карту",
                        StatusCode = StatusCode.OK,
                    };
                }
                else if (rentalrequest.ApprovalStatus != ApprovalStatus.TheBookingHasStarted && rentalrequest.ApprovalStatus != ApprovalStatus.TheUsersIsUnhappy && (rentalrequest.BookingStartDate - DateTime.UtcNow).TotalDays == 0 && ((rentalrequest.CheckInTime) - DateTime.Now.TimeOfDay).TotalHours >= 3)
                {
                    if (rentalrequest.PaymentActiveDepositId != null)
                    {
                        var paymentActiveDeposit = await _paymentRentalRequestRepository
                            .GetAll()
                            .FirstOrDefaultAsync(x => x.Id == rentalrequest.PaymentActiveDepositId);

                        if (paymentActiveDeposit != null && paymentActiveDeposit.Status == PaymentStatusDb.Succeeded)
                        {
                            await _paymentService.CreateRefundAsync(paymentActiveDeposit.Id!, 0);
                        }

                        rentalrequest.PaymentActiveDepositId = null;

                        rentalrequest.DeleteStatus = true;
                        rentalrequest.ApprovalStatus = ApprovalStatus.Rejected;
                        rentalrequest.DataChangeStatus = DateTime.UtcNow;
                        await _rentalRequestRepository.Update(rentalrequest);

                        return new BaseResponse<RentalRequestViewModel>()
                        {
                            Description = "Депозит скоро поступит на вашу карту, однако предоплата возвращена не будет",
                            StatusCode = StatusCode.OK,
                        };
                    }

                    rentalrequest.DeleteStatus = true;
                    rentalrequest.ApprovalStatus = ApprovalStatus.Rejected;
                    rentalrequest.DataChangeStatus = DateTime.UtcNow;
                    await _rentalRequestRepository.Update(rentalrequest);

                    return new BaseResponse<RentalRequestViewModel>()
                    {
                        Description = "Бронь отменена, предоплата возвращена не будет",
                        StatusCode = StatusCode.OK,
                    };
                }

                return new BaseResponse<RentalRequestViewModel>()
                {
                    Description = "Запрос на аренду не отменен",
                    StatusCode = StatusCode.Forbidden,
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

        public async Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTheLandlordIsUnhappyAdvertisementForUser(long id, long idUser, string description)
        {
            try
            {
                var rentalRequest = await _rentalRequestRepository.GetAll()
                    .Include(x => x.Advertisement)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (rentalRequest == null)
                {
                    return new BaseResponse<RentalRequestViewModel>
                    {
                        Description = "Запрос на аренду не найден",
                        StatusCode = StatusCode.RentalRequestNotFound
                    };
                }

                if (rentalRequest.Advertisement.IdAuthor != idUser)
                {
                    return new BaseResponse<RentalRequestViewModel>
                    {
                        Description = "Вы не являетесь владельцем объявления",
                        StatusCode = StatusCode.Forbidden
                    };
                }

                var conflict = await _conflictRepositoryRepository
                    .GetAll()
                    .FirstOrDefaultAsync(c => c.RentalRequestId == rentalRequest.Id && c.CreatedByUserId == idUser);

                if (rentalRequest.ApprovalStatus == ApprovalStatus.TheUsersIsUnhappy && conflict != null)
                {
                    return new BaseResponse<RentalRequestViewModel>
                    {
                        Description = $"Данная ситуация уже рассматривается, скоро с вами свяжутся",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                rentalRequest.ApprovalStatus = ApprovalStatus.TheUsersIsUnhappy;
                await _rentalRequestRepository.Update(rentalRequest);

                await _conflictRepositoryRepository.Create(new Conflict()
                {
                    RentalRequestId = rentalRequest.Id,
                    CreatedByUserId = idUser,
                    Description = description,
                    DateCreated = DateTime.UtcNow,
                    Status = ConflictStatus.Open
                });

                return new BaseResponse<RentalRequestViewModel>
                {
                    Data = new RentalRequestViewModel(
                    rentalRequest.Id,
                    rentalRequest.ApprovalStatus.GetDisplayName(),
                    rentalRequest.BookingStartDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalRequest.BookingFinishDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalRequest.DataChangeStatus.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalRequest.IdAuthorRentalRequest,
                    rentalRequest.IdNeedAdvertisement,
                    rentalRequest.Advertisement.IdAuthor,
                    rentalRequest.PaymentActiveId,
                    rentalRequest.PaymentActiveDepositId,
                    GetPrice(rentalRequest.BookingStartDate, rentalRequest.BookingFinishDate, rentalRequest.Advertisement.RentalPrice),
                    rentalRequest.FixedPrepaymentAmount,
                    rentalRequest.FixedDepositAmount,
                    rentalRequest.CheckInTime.ToString(@"hh\:mm"),
                    rentalRequest.CheckOutTime.ToString(@"hh\:mm"),
                    rentalRequest.IsBeforePhotosUploaded,
                    rentalRequest.IsAfterPhotosUploaded,
                    rentalRequest.IsPhotoSkippedByLandlord),
                    Description = "Запрос на отмену брони был направлен в тех поддержку, ждите скоро с вами свяжутся",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RentalRequestViewModel>
                {
                    Description = $"[CreateApprovalStatusTheLandlordIsUnhappyAdvertisementForUser]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTheTenantIsUnhappyAdvertisementForUser(long id, long idUser, string description)
        {
            try
            {
                var rentalRequest = await _rentalRequestRepository.GetAll()
                    .Include(x=>x.Advertisement)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (rentalRequest == null)
                {
                    return new BaseResponse<RentalRequestViewModel>
                    {
                        Description = "Запрос на аренду не найден",
                        StatusCode = StatusCode.RentalRequestNotFound
                    };
                }

                if (rentalRequest.IdAuthorRentalRequest != idUser)
                {
                    return new BaseResponse<RentalRequestViewModel>
                    {
                        Description = "Вы не являетесь автором запроса на аренду",
                        StatusCode = StatusCode.Forbidden
                    };
                }

                var conflict = await _conflictRepositoryRepository
                    .GetAll()
                    .FirstOrDefaultAsync(c => c.RentalRequestId == rentalRequest.Id && c.CreatedByUserId == idUser);

                if (rentalRequest.ApprovalStatus == ApprovalStatus.TheUsersIsUnhappy && conflict != null)
                {
                    return new BaseResponse<RentalRequestViewModel>
                    {
                        Description = $"Данная ситуация уже рассматривается, скоро с вами свяжутся",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                rentalRequest.ApprovalStatus = ApprovalStatus.TheUsersIsUnhappy;
                await _rentalRequestRepository.Update(rentalRequest);

                await _conflictRepositoryRepository.Create(new Conflict()
                {
                    RentalRequestId = rentalRequest.Id,
                    CreatedByUserId = idUser,
                    Description = description,
                    DateCreated = DateTime.UtcNow,
                    Status = ConflictStatus.Open
                });

                return new BaseResponse<RentalRequestViewModel>
                {
                    Data = new RentalRequestViewModel(
                    rentalRequest.Id,
                    rentalRequest.ApprovalStatus.GetDisplayName(),
                    rentalRequest.BookingStartDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalRequest.BookingFinishDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalRequest.DataChangeStatus.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalRequest.IdAuthorRentalRequest,
                    rentalRequest.IdNeedAdvertisement,
                    rentalRequest.Advertisement.IdAuthor,
                    rentalRequest.PaymentActiveId,
                    rentalRequest.PaymentActiveDepositId,
                    GetPrice(rentalRequest.BookingStartDate, rentalRequest.BookingFinishDate, rentalRequest.Advertisement.RentalPrice),
                    rentalRequest.FixedPrepaymentAmount,
                    rentalRequest.FixedDepositAmount,
                    rentalRequest.CheckInTime.ToString(@"hh\:mm"),
                    rentalRequest.CheckOutTime.ToString(@"hh\:mm"),
                    rentalRequest.IsBeforePhotosUploaded,
                    rentalRequest.IsAfterPhotosUploaded,
                    rentalRequest.IsPhotoSkippedByLandlord),
                    Description = "Запрос на отмену брони был направлен в тех поддержку, ждите скоро с вами свяжутся",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RentalRequestViewModel>
                {
                    Description = $"[CreateApprovalStatusTheTenantIsUnhappyAdvertisementForUser]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTheBookingHasStartedAdvertisementForUser(long id, long idUser)
        {
            try
            {
                var rentalRequest = await _rentalRequestRepository
                    .GetAll()
                    .Include(x => x.Advertisement)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (rentalRequest == null)
                {
                    return new BaseResponse<RentalRequestViewModel>
                    {
                        Description = "Запрос на аренду не найден",
                        StatusCode = StatusCode.RentalRequestNotFound
                    };
                }

                if (rentalRequest.IdAuthorRentalRequest != idUser)
                {
                    return new BaseResponse<RentalRequestViewModel>
                    {
                        Description = "Вы не являетесь владельцем объявления",
                        StatusCode = StatusCode.Forbidden
                    };
                }

                rentalRequest.ApprovalStatus = ApprovalStatus.TheBookingHasStarted;
                await _rentalRequestRepository.Update(rentalRequest);

                return new BaseResponse<RentalRequestViewModel>
                {
                    Data = new RentalRequestViewModel(
                    rentalRequest.Id,
                    rentalRequest.ApprovalStatus.GetDisplayName(),
                    rentalRequest.BookingStartDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalRequest.BookingFinishDate.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalRequest.DataChangeStatus.ToLocalTime().ToString("dd-MM-yyyy"),
                    rentalRequest.IdAuthorRentalRequest,
                    rentalRequest.IdNeedAdvertisement,
                    rentalRequest.Advertisement.IdAuthor,
                    rentalRequest.PaymentActiveId,
                    rentalRequest.PaymentActiveDepositId,
                    GetPrice(rentalRequest.BookingStartDate, rentalRequest.BookingFinishDate, rentalRequest.Advertisement.RentalPrice),
                    rentalRequest.FixedPrepaymentAmount,
                    rentalRequest.FixedDepositAmount,
                    rentalRequest.CheckInTime.ToString(@"hh\:mm"),
                    rentalRequest.CheckOutTime.ToString(@"hh\:mm"),
                    rentalRequest.IsBeforePhotosUploaded,
                    rentalRequest.IsAfterPhotosUploaded,
                    rentalRequest.IsPhotoSkippedByLandlord),
                    Description = "Статус обновлён: бронь началась",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RentalRequestViewModel>
                {
                    Description = $"[CreateApprovalStatusTheBookingHasStartedAdvertisementForUser]: {ex.Message}",
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

                model.BookingFinishDate = ((DateTime)model.BookingFinishDate).ToUniversalTime().Date;
                model.BookingStartDate = ((DateTime)model.BookingStartDate).ToUniversalTime().Date;

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
                    IsCalculated = false,
                    BookingStartDate = (DateTime)model.BookingStartDate!,
                    BookingFinishDate = (DateTime)model.BookingFinishDate!,
                    ApprovalStatus = ApprovalStatus.UnderСonsideration,
                    IdAuthorRentalRequest = IdAuthorRentalRequest,
                    IdNeedAdvertisement = model.IdNeedAdvertisement,
                    CheckInTime = model.CheckInTime,
                    CheckOutTime = model.CheckOutTime,
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
                    rentalrequest.PaymentActiveDepositId,
                    GetPrice(rentalrequest.BookingStartDate, rentalrequest.BookingFinishDate, rentalrequest.Advertisement.RentalPrice),
                    rentalrequest.FixedPrepaymentAmount,
                    rentalrequest.FixedDepositAmount,
                    rentalrequest.CheckInTime.ToString(@"hh\:mm"),
                    rentalrequest.CheckOutTime.ToString(@"hh\:mm"),
                    rentalrequest.IsBeforePhotosUploaded,
                    rentalrequest.IsAfterPhotosUploaded,
                    rentalrequest.IsPhotoSkippedByLandlord);

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
                                (rr.ApprovalStatus == ApprovalStatus.Completed || rr.ApprovalStatus == ApprovalStatus.Paid || rr.ApprovalStatus == ApprovalStatus.TheBookingHasStarted))
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

                query = query
                    .OrderByDescending(x => x.DataChangeStatus);

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
                    x.PaymentActiveDepositId,
                    GetPrice(x.BookingStartDate, x.BookingFinishDate, x.Advertisement.RentalPrice), x.FixedPrepaymentAmount,
                    x.FixedDepositAmount,
                    x.CheckInTime.ToString(@"hh\:mm"),
                    x.CheckOutTime.ToString(@"hh\:mm"),
                    x.IsBeforePhotosUploaded,
                    x.IsAfterPhotosUploaded,
                    x.IsPhotoSkippedByLandlord)).ToList();  

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
