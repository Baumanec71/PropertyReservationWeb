using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.Advertisement;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IAdvertisementService
    {
        Task<IBaseResponse<List<AdvertisementViewModel>>> GetMyAdvertisements(long id, int page);
        Task<IBaseResponse<List<AdvertisementViewModel>>> GetMyNoDeleteAdvertisements(long id, int page);
        Task<IBaseResponse<List<AdvertisementViewModel>>> GetConfirmationAdvertisements(int page);
        Task<IBaseResponse<PaginatedViewModelResponse<AdvertisementViewModel>>> GetAdvertisements(int page, AdvertisementFilterModel filterModel);
        Task<IBaseResponse<AdvertisementViewModel>> GetAdvertisement(long id);
        Task<IBaseResponse<AdvertisementViewModel>> DeleteAdvertisementForUser(long id);
        Task<IBaseResponse<AdvertisementViewModel>> CreateAdvertisement(CreateAdvertisementViewModel model);
        Task<IBaseResponse<AdvertisementViewModel>> Edit(AdvertisementViewModel model);
        Task<IBaseResponse<AdvertisementViewModel>> CalculatingTheRating(long id);
        Task<List<ObjectTypeOptionViewModel>> GetAllObjectTypes();
    }
}
