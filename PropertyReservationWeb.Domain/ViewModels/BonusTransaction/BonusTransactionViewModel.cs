namespace PropertyReservationWeb.Domain.ViewModels.BonusTransaction
{
    public record BonusTransactionViewModel(long Id, decimal Amount, string Description, string DateCreate, string Type, long UserId, long? ReviewId, long? AdvertisementId);
}
