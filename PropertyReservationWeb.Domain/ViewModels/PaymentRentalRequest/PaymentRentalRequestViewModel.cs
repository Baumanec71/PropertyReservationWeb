namespace PropertyReservationWeb.Domain.ViewModels.PaymentRentalRequest
{
    public record PaymentRentalRequestViewModel(string Id, long? RentalRequestId, decimal Amount, string? PaymentDate, string CreateDate, string Status, string Url);
}
