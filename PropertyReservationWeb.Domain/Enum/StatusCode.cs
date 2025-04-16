namespace PropertyReservationWeb.Domain.Enum
{
    public enum StatusCode
    {
        UserNotFound = 10,
        RentalRequestNotFound = 11,
        AdvertisementNotFound = 12,
        ReceiptNotFound = 13,
        PaymentNotFound = 14,
        ReviewNotFound = 15,
        BonusTransactionNotFound = 16,
        ConflictNotFound = 17,
        CreateReviewError = 34,
        Unauthorized = 401,
        UserAlreadyExists = 1,
        TheAdHasAlreadyBeenPosted = 40,
        MessageNotFound = 21,
        CreateRefundError = 700,
        CreateBookingPhotoError = 702,
        OK = 200,
        InternalServerError = 500,
        InvalidParameters = 500,
        ErorPassword = 220,
        ApprovalRequestNotFound = 43,
        DateBooked = 30,
        PaymentError = 1001,
        Forbidden = 707,
    }
}
