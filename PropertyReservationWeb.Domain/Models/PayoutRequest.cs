using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.Models
{
    public class PayoutRequest
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public User User { get; set; } = null!;
        public decimal Amount { get; set; }
        public PaymentStatusDb Status { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
