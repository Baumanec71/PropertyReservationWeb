namespace PropertyReservationWeb.Domain.Models
{
    public class AdvertisementAmenity
    {
        private AdvertisementAmenity() { }
        public AdvertisementAmenity(long idAdvertisement, long idAmenity, bool isActive, int? value)
        {
            IdAdvertisement = idAdvertisement;
            IdAmenity = idAmenity;
            IsActive = isActive;
            Value = value;
        }
        public long Id { get; set; }
        public long IdAdvertisement { get;set; }
        public long IdAmenity { get; set; }
        public bool IsActive { get; set; }
        public int? Value { get; set; }
        public Advertisement Advertisement { get; set; } = null!;
        public Amenity Amenity { get; set; } = null!;
    }
}
