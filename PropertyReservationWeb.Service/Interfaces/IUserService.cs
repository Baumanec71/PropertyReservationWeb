using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.User;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IUserService
    {
        Task<IBaseResponse<UserViewModel>> GetUserId(long id);
        Task<IBaseResponse<User>> CalculatingTheBonusPointsUser(long id);
        Task<IBaseResponse<UserViewModel>> GetUserEmail(string email);
        Task<IBaseResponse<PaginatedViewModelResponse<UserViewModel, UserFilterViewModel>>> GetUsers(int page);
        Task<IBaseResponse<UserViewModel>> DeleteUser(long id);
        Task<BaseResponse<UserViewModel>> Update(UpdateUserViewModel model, long id);
        BaseResponse<Dictionary<int, string>> GetRoles();
        Task<IBaseResponse<User>> CalculatingTheRatingUser(long id);
    }
}
