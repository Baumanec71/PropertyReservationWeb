namespace PropertyReservationWeb.Domain.ViewModels.RentalRequest
{
    public record PaginatedRentalViewModelResponse<T>(List<T> ViewModels, int TotalPages, RentalRequestFilterModel? filterModel);
}
