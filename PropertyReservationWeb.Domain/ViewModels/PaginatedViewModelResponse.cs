namespace PropertyReservationWeb.Domain.ViewModels
{
    public record PaginatedViewModelResponse<T, M>(List<T> ViewModels, int TotalPages, M? filterModel);
}
