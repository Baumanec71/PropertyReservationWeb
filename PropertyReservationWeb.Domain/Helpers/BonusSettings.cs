namespace PropertyReservationWeb.Domain.Helpers
{
    public class BonusSettings
    {
        public decimal RenterBonusPerDay { get; set; } = 1.0m;
        public decimal LandlordBonusPerDay { get; set; } = 0.8m;

        public Dictionary<int, decimal> RatingMultipliers { get; set; } = new()
        {
            { 5, 1.5m },
            { 4, 1.2m },
            { 3, 1.0m },
            { 2, 0.7m },
            { 1, 0.5m }
        };
    }
}
