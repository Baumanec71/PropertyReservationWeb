using Microsoft.AspNetCore.Mvc.ApplicationModels;
using PropertyReservationWeb.Domain.ViewModels.Advertisement;

namespace PropertyReservationWeb.Domain.ViewModels
{
    public record PaginatedViewModelResponse<T>(List<T> ViewModels, int TotalPages, AdvertisementFilterModel? filterModel);
}
