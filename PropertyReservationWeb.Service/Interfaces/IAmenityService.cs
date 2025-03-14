using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IAmenityService
    {
        Task<List<AmenityType>> GetAllAmenityTypes();
    }
}
