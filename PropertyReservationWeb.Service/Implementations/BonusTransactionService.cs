using Microsoft.Diagnostics.Tracing.StackSources;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.BonusTransaction;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PropertyReservationWeb.Service.Implementations
{
    public class BonusTransactionService : IBonusTransactionService
    {
        private const int pageSize = 10;
        private readonly IBaseRepository<BonusTransaction> _bonusTransactionRepository;

        public BonusTransactionService(IBaseRepository<BonusTransaction> bonusTransactionRepository)
        {
            _bonusTransactionRepository = bonusTransactionRepository;
        }

        public async Task<IBaseResponse<PaginatedViewModelResponse<BonusTransactionViewModel, BonusTransactionFilterModel>>> GetBonusTransactions(int page, BonusTransactionFilterModel filterModel)
        {
            try
            {
                var query = _bonusTransactionRepository.GetAll();

                if (!string.IsNullOrEmpty(filterModel.Type))
                {
                    query = query.Where(bt => bt.Type == filterModel.Type);
                }

                if (filterModel.UserId.HasValue)
                {
                    query = query.Where(bt => bt.UserId == filterModel.UserId.Value);
                }

                var totalRecords = await query.CountAsync();

                var transactions = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModels = transactions
                    .Select(t => new BonusTransactionViewModel(
                        t.Id,
                        t.Amount,
                        t.Description,
                        t.DateCreate.ToLocalTime().ToString("dd-MM-yyyy"),
                        t.Type,
                        t.UserId,
                        t.ReviewId,
                        t.AdvertisementId))
                    .ToList();

                var response = new PaginatedViewModelResponse<BonusTransactionViewModel, BonusTransactionFilterModel>(
                    viewModels,
                    (int)Math.Ceiling(totalRecords / (double)pageSize),
                    filterModel);

                return new BaseResponse<PaginatedViewModelResponse<BonusTransactionViewModel, BonusTransactionFilterModel>>
                {
                    Data = response,
                    StatusCode = StatusCode.OK,
                    Description = "Успешно получены бонусные транзакции с пагинацией"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginatedViewModelResponse<BonusTransactionViewModel, BonusTransactionFilterModel>>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Ошибка при получении бонусных транзакций: {ex.Message}"
                };
            }
        }

        public async Task<IBaseResponse<BonusTransactionViewModel>> CreateBonusTransaction(CreateBonusTransactionViewModel model, long idUser)
        {
            try
            {
                var transaction = new BonusTransaction
                {
                    Amount = model.Amount,
                    Description = model.Description,
                    IsCalculated = false,
                    DateCreate = DateTime.UtcNow,
                    Type = model.Type,
                    UserId = idUser,
                    ReviewId = model.ReviewId,
                    AdvertisementId = model.AdvertisementId
                };

                await _bonusTransactionRepository.Create(transaction);

                var result = new BonusTransactionViewModel(
                    transaction.Id,
                    transaction.Amount,
                    transaction.Description,
                    transaction.DateCreate.ToLocalTime().ToString("dd-MM-yyyy"),
                    transaction.Type,
                    transaction.UserId,
                    transaction.ReviewId,
                    transaction.AdvertisementId);

                return new BaseResponse<BonusTransactionViewModel>
                {
                    Data = result,
                    StatusCode = StatusCode.OK,
                    Description = "Бонусная транзакция успешно создана"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<BonusTransactionViewModel>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Ошибка при создании бонусной транзакции: {ex.Message}"
                };
            }
        }

        public async Task<IBaseResponse<BonusTransactionViewModel>> DeleteBonusTransaction(long id)
        {
            try
            {
                var transaction = await _bonusTransactionRepository
                    .GetAll()
                    .FirstOrDefaultAsync(bt => bt.Id == id);

                if (transaction == null)
                {
                    return new BaseResponse<BonusTransactionViewModel>
                    {
                        StatusCode = StatusCode.BonusTransactionNotFound,
                        Description = "Транзакция не найдена"
                    };
                }

                await _bonusTransactionRepository.Delete(transaction);

                return new BaseResponse<BonusTransactionViewModel>
                {
                    StatusCode = StatusCode.OK,
                    Description = "Бонусная транзакция успешно удалена"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<BonusTransactionViewModel>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Ошибка при удалении бонусной транзакции: {ex.Message}"
                };
            }
        }
    }
}
