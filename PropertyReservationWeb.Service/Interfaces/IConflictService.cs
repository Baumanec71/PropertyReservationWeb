using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.Conflict;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IConflictService
    {
        Task<IBaseResponse<PaginatedViewModelResponse<ConflictViewModel, ConflictFilterModel>>> GetConflicts(int page, ConflictFilterModel filterModel);
        Task<IBaseResponse<ConflictViewModel>> ResolvedConflict(long id,long idUser);
        Task<IBaseResponse<ConflictViewModel>> RejectedConflict(long id, long idUser);
    }
}
