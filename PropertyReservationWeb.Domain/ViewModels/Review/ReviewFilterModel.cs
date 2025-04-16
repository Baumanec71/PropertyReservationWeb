namespace PropertyReservationWeb.Domain.ViewModels.Review
{
    public class ReviewFilterModel
    {
        public long? SelectedIdAuthor { get; set; }
        public long? SelectedIdUserWhoReview{ get; set; }
        public long? SelectedIdAdvertisement { get; set; }
        public bool? SelectedDeleteStatus { get; set; }
        public int? SelectedTheQualityOfTheTransaction { get; set; }
        public DateTime? SelectedDate { get; set; }
        public bool? IsAuthor {  get; set; }
    }
}
