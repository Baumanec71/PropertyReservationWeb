using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.BonusTransaction;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IBonusTransactionService
    {
        Task<IBaseResponse<PaginatedViewModelResponse<BonusTransactionViewModel, BonusTransactionFilterModel>>> GetBonusTransactions(int page, BonusTransactionFilterModel filterModel);
        Task<IBaseResponse<BonusTransactionViewModel>> CreateBonusTransaction(CreateBonusTransactionViewModel model, long IdUser);
        Task<IBaseResponse<BonusTransactionViewModel>> DeleteBonusTransaction(long id);
    }
}
