using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.RentalRequest;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IRentalRequestService
    {
        Task<IBaseResponse<PaginatedRentalViewModelResponse<RentalRequestViewModel>>> GetRentalRequests(int page, RentalRequestFilterModel filterModel);
        Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTrueAdvertisementForUser(long id, long idUser);
        Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusFalseAdvertisementForUser(long id, long idUser);
        Task<IBaseResponse<RentalRequestViewModel>> GetRentalRequest(long id);
        Task<IBaseResponse<RentalRequestViewModel>> DeleteRentalRequestForUser(long id, long idUser);
        Task<IBaseResponse<List<DateTime>>> GetAllBookedDates(long id);
      //  Task<IBaseResponse<RentalRequestViewModel>> DeleteAdvertisementForAdmin(long id);
        Task<IBaseResponse<RentalRequestViewModel>> CreateRentalRequest(CreateRentalRequestViewModel model, long IdAuthorRentalRequest);
        Task<IBaseResponse<RentalRequestViewModel>> DeleteRentalRequests();
        Task<List<ApprovalStatusOptionViewModel>> GetAllApprovalStatus();
    }
}
