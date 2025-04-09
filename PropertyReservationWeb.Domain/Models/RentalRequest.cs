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
        public DateTime DataChangeStatus { get; set; }
        public long IdAuthorRentalRequest { get; set; }
        public User User { get; set; } = null!;
        public long IdNeedAdvertisement { get; set; }
        public string? PaymentActiveId { get; set; } = string.Empty;
        public Advertisement Advertisement { get; set; } = null!;
        public List<Review> Reviews { get; set; } = new();
        public List<PaymentRentalRequest> Payments { get; set; } = new();

        public RentalRequest()
        {
        }
    }
}
