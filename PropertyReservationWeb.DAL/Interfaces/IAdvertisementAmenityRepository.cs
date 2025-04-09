using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Interfaces
{
    public interface IAdvertisementAmenityRepository
    {
        Task CreateRange(List<AdvertisementAmenity> entity);
        Task<List<AdvertisementAmenity>> UpdateRange(List<AdvertisementAmenity> entity);
    }
}
