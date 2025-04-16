using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.Models
{
    public class ReservationChangeRequest
    {
        public long Id { get; set; }
        public long RentalRequestId { get; set; }
        public RentalRequest RentalRequest { get; set; } = null!;
        public DateTime NewStartDate { get; set; }
        public DateTime NewFinishDate { get; set; }
        public decimal NewFixedPrepaymentAmount { get; set; }
        public decimal NewFixedDepositAmount { get; set; }
        public ReservationChangeStatus Status { get; set; }
        public long RequestedByUserId { get; set; }
        public DateTime CreateDate { get; set; }
       // public List<PaymentRentalRequest> RefundedPayments { get; set; } = new();
        public List<PaymentRentalRequest> NewPayments { get; set; } = new();
    }
}
