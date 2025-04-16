using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.BookingPhoto;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IBookingPhotoService
    {
        Task<IBaseResponse<List<CreateBookingPhotoViewModel>>> AddPhotoAsync(List<CreateBookingPhotoViewModel> photos, long rentalRequestId, long idUser );
        Task<IBaseResponse<(List<BookingPhotoViewModel>, List<BookingPhotoViewModel>)>> GetPhotosAsync(long rentalRequestId, bool? isBefore = null);
    }
}
