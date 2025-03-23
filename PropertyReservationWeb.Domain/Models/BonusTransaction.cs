namespace PropertyReservationWeb.Domain.Models
{
    public class BonusTransaction
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public int Amount { get; set; } // количество
        public bool IsEarned { get; set; } //начислены или потрачены
        public string Description { get; set; } = string.Empty;
        public DateTime DateCreate { get; set; }
    }
}
