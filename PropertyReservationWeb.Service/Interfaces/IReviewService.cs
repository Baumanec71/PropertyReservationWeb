using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.Review;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IReviewService
    {
        Task<IBaseResponse<PaginatedViewModelResponse<ReviewViewModel, ReviewFilterModel>>> GetReviews(int page, ReviewFilterModel filterModel);
        Task<IBaseResponse<ReviewViewModel>> CreateReview(CreateReviewViewModel model, long IdUser);
        Task<IBaseResponse<ReviewViewModel>> DeleteReview(long id);
    }
}
