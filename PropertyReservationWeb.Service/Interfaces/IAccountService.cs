using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.Account;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IAccountService
    {
        Task<IBaseResponse<string>> Login(LoginViewModel model);
        Task<IBaseResponse<bool>> Logout(string email);
        Task<IBaseResponse<bool>> ChangePassword(ChangePasswordViewModel model, string email);
        Task<IBaseResponse<string>> Register(RegisterViewModel model);
    }
}
