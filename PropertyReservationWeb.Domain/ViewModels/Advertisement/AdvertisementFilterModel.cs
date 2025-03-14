using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.ViewModels.Amenity;
using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.Advertisement
{
    public class AdvertisementFilterModel
    {
        public string? SelectedAddress { get; set; }
        public ObjectType? SelectedObjectType { get; set; }
        public uint? SelectedTotalArea { get; set; }
        public decimal? SelectedMinRentalPrice { get; set; }
        public decimal? SelectedMaxRentalPrice { get; set; }
        public decimal? SelectedMaxFixedPrepaymentAmount { get; set; }
        public decimal? SelectedMinFixedPrepaymentAmount { get; set; }
        public uint? SelectedNumberOfRooms { get; set; }
        public uint? SelectedNumberOfBeds { get; set; }
        public uint? SelectedNumberOfBathrooms { get; set; }
        public double? SelectedMinRating { get; set; }
        public uint? SelectedNumberOfPromotionPoints { get; set; }
        public bool? SelectedConfirmationStatus { get; set; }
        public List<ObjectTypeOptionViewModel> types { get; set; } = new();
        public List<CreateAdvertisementAmenityViewModel> CreateAdvertisementAmenities { get; set; } = new();
    }
}
