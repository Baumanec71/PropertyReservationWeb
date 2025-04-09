namespace PropertyReservationWeb.Domain.ViewModels.Review
{
    public record ReviewViewModel(long Id, int TheQualityOfTheTransaction, string Comment, string DateOfCreation, long IdNeedRentalRequest, long IdAuthorReviw, long IdWhoReview);
}
