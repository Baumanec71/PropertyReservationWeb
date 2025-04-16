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
        public bool IsCalculated {  get; set; }
        public User User { get; set; } = null!;
        public long IdNeedAdvertisement { get; set; }
        public decimal FixedPrepaymentAmount { get; set; }
        public decimal FixedDepositAmount { get; set; }
        public List<BookingPhoto> BookingPhotos { get; set; } = new();
        public string? PaymentActiveId { get; set; } = string.Empty;
        public string? PaymentActiveDepositId { get; set; } = string.Empty;
        public bool IsBeforePhotosUploaded { get; set; }
        public bool IsAfterPhotosUploaded { get; set; }
        public bool IsPhotoSkippedByLandlord { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public Advertisement Advertisement { get; set; } = null!;
        public long? ReservationChangeRequestId {  get; set; }
        public ReservationChangeRequest ReservationChangeRequest { get; set; } = null!;
        public List<Review> Reviews { get; set; } = new();
        public List<PaymentRentalRequest> Payments { get; set; } = new();

        public RentalRequest()
        {
        }
    }
}
