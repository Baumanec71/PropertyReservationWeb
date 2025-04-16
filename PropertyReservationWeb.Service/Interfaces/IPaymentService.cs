using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.PaymentRentalRequest;
using Yandex.Checkout.V3;
using static PropertyReservationWeb.Service.Implementations.PaymentService;

namespace PropertyReservationWeb.Service.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(decimal amount, string description, string returnUrl);
        Task<IBaseResponse<PaymentRentalRequestViewModel>> GetPaymentRentalRequest(string id);
        Task<PaymentResponse<YooKassaResponse>> GetYooKassaResponse(string paymentRequestId);
        Task<BaseResponse<Refund>> CreateRefundAsync(string id, double shtraf);
        Task MarkPaymentAsSucceeded(string paymentId);
        Task MarkPaymentAsCanceled(string paymentId);
        Task MarkRefundAsSucceeded(string refundId);
    }
}
