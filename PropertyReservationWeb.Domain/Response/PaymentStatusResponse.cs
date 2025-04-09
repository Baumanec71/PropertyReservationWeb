namespace PropertyReservationWeb.Domain.Response
{
    public class PaymentResponse<T>
    {
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
