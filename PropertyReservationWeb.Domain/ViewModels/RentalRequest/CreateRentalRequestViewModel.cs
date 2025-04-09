namespace PropertyReservationWeb.Domain.ViewModels.RentalRequest
{
    public class CreateRentalRequestViewModel
    {
        public DateTime? BookingStartDate { get; set; } = null!;
        public DateTime? BookingFinishDate { get; set; } = null!;
        public long IdNeedAdvertisement { get; set; }
        public List<DateTime> BookedDates { get; set; } = new();
        public decimal RentalPrice { get; set; }
    }
}
