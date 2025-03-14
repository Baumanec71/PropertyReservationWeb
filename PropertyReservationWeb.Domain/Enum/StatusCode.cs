namespace PropertyReservationWeb.Domain.Enum
{
    public enum StatusCode
    {
        UserNotFound = 10,
        Unauthorized = 401,
        UserAlreadyExists = 1,
        RentalRequestNotFound = 30,
        AdvertisementNotFound = 10,
        ReceiptNotFound = 20,
        TheAdHasAlreadyBeenPosted = 40,
        MessageNotFound = 21,
        OK = 200,
        InternalServerError = 500,
        InvalidParameters = 500,
        ErorPassword = 220,
    }
}
