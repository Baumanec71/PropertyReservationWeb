namespace PropertyReservationWeb.Domain.Models
{
    public class BonusTransaction
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DateCreate { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsCalculated { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = new();
        public long? ReviewId { get; set; }
        public Review? Review { get; set; } = null;
        public long? AdvertisementId { get; set; }
        public Advertisement? Advertisement { get; set; } = null;
    }
}
