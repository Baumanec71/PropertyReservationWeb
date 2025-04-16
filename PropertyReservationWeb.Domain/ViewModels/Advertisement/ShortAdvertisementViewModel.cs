using PropertyReservationWeb.Domain.ViewModels.Convenience;
using PropertyReservationWeb.Domain.ViewModels.Photo;

namespace PropertyReservationWeb.Domain.ViewModels.Advertisement
{
    public record ShortAdvertisementViewModel(
        long Id,
        string ObjectType,
        string AdressName,
        string Description,
        uint TotalArea,
        decimal RentalPrice,
        double Rating,
        int NumberOfTransactions,
        uint NumberOfRooms,
        uint NumberOfBeds,
        uint NumberOfBathrooms,
        long IdAuthor,
        List<PhotoViewModel> Photos,
        List<AmenityViewModel> Amenityes
        );
}
