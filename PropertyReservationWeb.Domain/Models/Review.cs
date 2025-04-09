namespace PropertyReservationWeb.Domain.Models
{
    public class Review
    {
        public Review() { }
        public Review(
            int theQualityOfTheTransaction,
            string comment,
            bool statusDel,
            DateTime dateOfCreation,
            long idNeedRentalRequest,
            bool isTheLandlord, bool isCalculated) 
        { 
            TheQualityOfTheTransaction = theQualityOfTheTransaction;
            Comment = comment;
            StatusDel = statusDel;
            DateOfCreation = dateOfCreation;
            IdNeedRentalRequest = idNeedRentalRequest;
            IsTheLandlord = isTheLandlord;
            IsCalculated = isCalculated;
        }
        public long Id { get; set; }
        public int TheQualityOfTheTransaction { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool StatusDel { get; set; }
        public bool IsCalculated { get; set; }
        public DateTime DateOfCreation { get; set; }
        public bool IsTheLandlord { get; set; }
        public long IdNeedRentalRequest { get; set; }
        public RentalRequest RentalRequest { get; set; }
        public List<BonusTransaction> BonusTransactions { get; set; } = new();
    }
}
