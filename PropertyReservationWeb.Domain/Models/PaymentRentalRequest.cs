using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.Models
{
    public class PaymentRentalRequest
    {
        public PaymentRentalRequest()
        {
        }
        public string? Id { get; set; } = string.Empty;
        public long? RentalRequestId { get; set; } = null!;
        public RentalRequest RentalRequest { get; set; } = null!;
        public decimal Amount { get; set; }
        public PaymentStatusDb Status { get; set; }
        public DateTime? PaymentDate { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
