using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Service.Interfaces;

namespace PropertyReservationWeb.Service.Implementations
{
    public class AmenityService : IAmenityService
    {
        private readonly IBaseRepository<Amenity> _amenityRepository;

        public AmenityService(IBaseRepository<Amenity> amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<List<AmenityType>> GetAllAmenityTypes()
        {
            var amenities = await _amenityRepository
                .GetAll()
                .AsNoTracking()
                .Select(a => a.AmenityType)
                .ToListAsync();

            return amenities;
        }

    }
}
