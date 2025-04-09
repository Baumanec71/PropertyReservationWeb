namespace PropertyReservationWeb.Domain.ViewModels.BonusTransaction
{
    public class CreateBonusTransactionViewModel
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public long? ReviewId { get; set; }
        public long? AdvertisementId { get; set; }
    }
}
