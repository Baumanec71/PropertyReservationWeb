using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.RentalRequest;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IRentalRequestService
    {
        Task<IBaseResponse<PaginatedRentalViewModelResponse<RentalRequestViewModel>>> GetRentalRequests(int page, RentalRequestFilterModel filterModel);
        Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTrueAdvertisementForUser(long id, long idUser, decimal FixedPrepaymentAmount, decimal FixedDepositAmount, bool IsPhotoSkippedByLandlord);
        Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusFalseAdvertisementForUser(long id, long idUser);
        Task<IBaseResponse<List<RentalRequestViewModel>>> UpdateRentalStatusComplete();
        Task<IBaseResponse<RentalRequestViewModel>> GetRentalRequest(long id);
        Task<IBaseResponse<RentalRequestViewModel>> DeleteRentalRequestForUser(long id, long idUser);
        Task<IBaseResponse<List<DateTime>>> GetAllBookedDates(long id);
      //  Task<IBaseResponse<RentalRequestViewModel>> DeleteAdvertisementForAdmin(long id);
        Task<IBaseResponse<RentalRequestViewModel>> CreateRentalRequest(CreateRentalRequestViewModel model, long IdAuthorRentalRequest);
        Task<IBaseResponse<RentalRequestViewModel>> DeleteRentalRequests();
        Task<List<ApprovalStatusOptionViewModel>> GetAllApprovalStatus();
        Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTheBookingHasStartedAdvertisementForUser(long id, long idUser);
        Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTheTenantIsUnhappyAdvertisementForUser(long id, long idUser, string description);
        Task<IBaseResponse<RentalRequestViewModel>> CreateApprovalStatusTheLandlordIsUnhappyAdvertisementForUser(long id, long idUser, string description);
    }
}
