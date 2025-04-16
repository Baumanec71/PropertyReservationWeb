namespace PropertyReservationWeb.Domain.ViewModels.RentalRequest
{
    public record class RentalRequestViewModel(
        long Id,
        string ApprovalStatus,
        string BookingStartDate,
        string BookingEndDate,
        string DataChangeStatus,
        long IdAuthorRentalRequest,
        long IdNeedAdvertisement,
        long IdAuthorAdvertisement,
        string? PaymentId,
        string? PaymentActiveDepositId,
        decimal Price, 
        decimal FixedPrepaymentAmount,
        decimal FixedDepositAmount,
        string CheckInTime,
        string CheckOutTime,
        bool IsBeforePhotosUploaded,
        bool IsAfterPhotosUploaded,
        bool IsPhotoSkippedByLandlord);
}
