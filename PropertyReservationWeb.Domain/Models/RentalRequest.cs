using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.Models
{
    public class RentalRequest
    {
        public long Id { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public bool DeleteStatus { get; set; }
        public DateTime BookingStartDate { get; set; }
        public DateTime BookingFinishDate { get; set; }
        public bool RecipientsViewingStatus { get; set; }
        public bool AuthorsViewingStatus { get; set; }
        public DateTime DataChangeStatus { get; set; }
        public long IdAuthorRentalRequest { get; set; }
        public User User { get; set; } = null!;
        public long IdNeedAdvertisement { get; set; }
        public Advertisement Advertisement { get; set; } = null!;
        public List<Review> Review { get; set; } = new();

        public RentalRequest()
        {
        }
    }
}
