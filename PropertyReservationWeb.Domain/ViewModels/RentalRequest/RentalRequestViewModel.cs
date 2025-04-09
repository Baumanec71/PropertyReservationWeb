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
        decimal Price);
}
