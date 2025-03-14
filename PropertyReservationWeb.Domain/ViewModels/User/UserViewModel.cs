namespace PropertyReservationWeb.Domain.ViewModels.User
{
    public record UserViewModel(long Id,
            string Password,
            string Email,
            string Role,
            string Name,
            bool DeleteStatus,
            double Rating,
            string PhoneNumber,
            string DateOfRegistration,
            string? avatar,
            int NumberOfAdsCreated,
            int NumberOfTransactions
        );
}
