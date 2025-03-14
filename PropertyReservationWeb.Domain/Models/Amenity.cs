using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.Models
{
    public class Amenity
    {
        public Amenity() { }
        public long Id { get; set; }
        public AmenityType AmenityType { get; set; }
        public List<AdvertisementAmenity> AdvertisementAmenity { get; set; } = new();
    }
}
