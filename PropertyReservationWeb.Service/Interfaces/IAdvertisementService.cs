using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.Advertisement;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IAdvertisementService
    {
        Task<IBaseResponse<PaginatedViewModelResponse<T, AdvertisementFilterModel>>> GetAdvertisements<T>(int page, AdvertisementFilterModel filterModel, bool? selectedConfirmationStatus, long? idAuthor, bool? selectedDeleteStatus);
        Task<IBaseResponse<CreateAdvertisementViewModel>> GetAdvertisementByIdCreateModel(long id);
        Task<IBaseResponse<AdvertisementViewModel>> CreateConfirmationStatusTrueAdvertisementForAdmin(long id);
        Task<IBaseResponse<AdvertisementViewModel>> CreateConfirmationStatusFalseAdvertisementForAdmin(long id);
        Task<IBaseResponse<AdvertisementViewModel>> GetAdvertisement(long id);
        Task<IBaseResponse<AdvertisementViewModel>> DeleteAdvertisementForUser(long id, long idUser);
        Task<IBaseResponse<AdvertisementViewModel>> DeleteAdvertisementForAdmin(long id);
        Task<IBaseResponse<AdvertisementViewModel>> CreateAdvertisement(CreateAdvertisementViewModel model);
        Task<IBaseResponse<CreateAdvertisementViewModel>> Edit(CreateAdvertisementViewModel model, long id);
        Task<IBaseResponse<AdvertisementViewModel>> CalculatingTheRating(long id);
        List<ObjectTypeOptionViewModel> GetAllObjectTypes();
    }
}
