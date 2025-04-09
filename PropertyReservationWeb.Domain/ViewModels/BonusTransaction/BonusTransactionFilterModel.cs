namespace PropertyReservationWeb.Domain.ViewModels.BonusTransaction
{
    public class BonusTransactionFilterModel
    {
        public string? Type { get; set; }
        public long? UserId { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
