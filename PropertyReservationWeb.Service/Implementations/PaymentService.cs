using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Extensions;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.PaymentRentalRequest;
using PropertyReservationWeb.Service.Interfaces;
using System.Net.Http.Headers;
using Yandex.Checkout.V3;
using System.Text;
using System.Text.Json;

namespace PropertyReservationWeb.Service.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly Client _client;
        private readonly IBaseRepository<PaymentRentalRequest> _paymentRentalRequestRepository;
        private readonly IBaseRepository<RentalRequest> _rentalRequestRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaymentService(IBaseRepository<PaymentRentalRequest> paymentRentalRequestRepository, IBaseRepository<RentalRequest> rentalRequestRepository, IBaseRepository<User> userRepository, HttpClient httpClient, IConfiguration configuration)
        {
            _rentalRequestRepository = rentalRequestRepository;
            _userRepository = userRepository;
            _paymentRentalRequestRepository = paymentRentalRequestRepository;
            _httpClient = httpClient;
            _configuration = configuration;
            _client = new Client(
                configuration["YooKassa:ShopId"],
                configuration["YooKassa:SecretKey"]);
        }

        public async Task<Payment> CreatePaymentAsync(decimal amount, string description, string returnUrl)
        {
            var payment = new Payment
            {
                Amount = new Amount { Value = amount, Currency = "RUB" },
                Confirmation = new Confirmation { Type = ConfirmationType.Redirect, ReturnUrl = returnUrl },
                Capture = true,
                Description = description,
                Id = Guid.NewGuid().ToString(),
                PaymentMethodData = new PaymentMethod
                {
                    Type = PaymentMethodType.BankCard
                }
            };

            var idempotenceKey = payment.Id;

            return await _client.MakeAsync().CreatePaymentAsync(payment, idempotenceKey);
        }

        public async Task<BaseResponse<Refund>> CreateRefundAsync(string id, bool shtraf)
        {
            try
            {
                var payment = await _paymentRentalRequestRepository
               .GetAll()
               .FirstOrDefaultAsync(prr => prr.Id == id);

                if (payment == null)
                {
                    return new BaseResponse<Refund>
                    {
                        StatusCode = StatusCode.PaymentNotFound,
                        Description = "Не найден платеж для возврата"
                    };
                }

                var value = payment.Amount;

                if (shtraf == true)
                {
                    value = payment.Amount * 0.85m;
                }

                var refund = new Refund
                {
                    Amount = new Amount { Value = value, Currency = "RUB" },
                    PaymentId = payment.Id,
                };

                var createfullrefund = await _client.MakeAsync().CreateRefundAsync(refund);

                return new BaseResponse<Refund>
                {
                    Data = createfullrefund,
                    StatusCode = StatusCode.OK,
                    Description = "Деньги возвращены"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Refund>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Не удалось создать запрос на возврат денежных средств {ex}"
                };
            }  
        }

        public async Task<PaymentResponse<YooKassaResponse>> GetYooKassaResponse(string paymentRequestId)
        {
            try
            {
                string url = $"https://api.yookassa.ru/v3/payments/{paymentRequestId}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_configuration["YooKassa:ShopId"]}:{_configuration["YooKassa:SecretKey"]}"))
                );

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return new PaymentResponse<YooKassaResponse>
                    {
                        ErrorMessage = $"Ошибка запроса: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}"
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ответ сервера: {content}");

                var result = System.Text.Json.JsonSerializer.Deserialize<YooKassaResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null)
                {
                    return new PaymentResponse<YooKassaResponse>
                    {
                        ErrorMessage = "Ошибка обработки JSON"
                    };
                }

                return new PaymentResponse<YooKassaResponse>
                {
                    Data = result,
                    ErrorMessage = null
                };
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                return new PaymentResponse<YooKassaResponse>
                {
                    ErrorMessage = $"Ошибка парсинга JSON: {jsonEx.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse<YooKassaResponse>
                {
                    ErrorMessage = $"Ошибка: {ex.Message}"
                };
            }
        }

        public class YooKassaResponse
        {
            public string Id { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public AmountYooKassa? Amount { get; set; }
        }

        public class AmountYooKassa
        {
            public string Value { get; set; } = string.Empty;
            public string Currency { get; set; } = string.Empty;
        }

        public async Task<IBaseResponse<PaymentRentalRequestViewModel>> GetPaymentRentalRequest(string id)
        {
            try 
            {
                var paymentRentalRequest = await _paymentRentalRequestRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if(paymentRentalRequest == null)
                {
                    return new BaseResponse<PaymentRentalRequestViewModel>()
                    {
                        Description = "Запрос на оплату не найден",
                        StatusCode = StatusCode.PaymentNotFound,
                    };
                }

                var PaymentDateView = paymentRentalRequest.PaymentDate.ToString();
                var CreateDateView = ((DateTime)(paymentRentalRequest.CreateDate)).ToLocalTime().ToString("dd-MM-yyyy");

                if (paymentRentalRequest.PaymentDate != null)
                {
                    PaymentDateView = ((DateTime)(paymentRentalRequest.PaymentDate)).ToLocalTime().ToString("dd-MM-yyyy");
                }

                var view = new PaymentRentalRequestViewModel
                    (
                    paymentRentalRequest.Id!,
                    paymentRentalRequest.RentalRequestId,
                    paymentRentalRequest.Amount,
                    PaymentDateView,
                    CreateDateView,
                    paymentRentalRequest.Status.GetDisplayName(),
                    paymentRentalRequest.Url
                    );

                return new BaseResponse<PaymentRentalRequestViewModel>()
                {
                    Data = view,
                    Description = "Запрос на аренду найден",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaymentRentalRequestViewModel>()
                {
                    Description = $"[GetPaymentRentalRequest]:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task MarkPaymentAsSucceeded(string paymentId)
        {
            var payment = await _paymentRentalRequestRepository
                .GetAll()
                .Include(x=>x.RentalRequest)
                .ThenInclude(rr=>rr.Advertisement)
                .ThenInclude(a=>a.User)
                .FirstOrDefaultAsync(x => x.Id == paymentId);

            if (payment == null)
            {
                return;
            }

            payment.Status = PaymentStatusDb.Succeeded;
            payment.PaymentDate = DateTime.UtcNow;
            await _paymentRentalRequestRepository.Update(payment);

            payment.RentalRequest.ApprovalStatus = ApprovalStatus.Paid;
            payment.RentalRequest.DataChangeStatus = DateTime.UtcNow;
            await _rentalRequestRepository.Update(payment.RentalRequest);

            payment.RentalRequest.Advertisement.User.Balance += payment.Amount * 0.85m;
            await _userRepository.Update(payment.RentalRequest.Advertisement.User);
        }

        public async Task MarkRefundSucceeded(string paymentId)
        {
            var payment = await _paymentRentalRequestRepository
              .GetAll()
              .FirstOrDefaultAsync(x => x.Id == paymentId);

            if (payment == null)
            {
                return;
            }

            payment.Status = PaymentStatusDb.Refund;
            await _paymentRentalRequestRepository.Update(payment);
        }

        public async Task MarkPaymentAsCanceled(string paymentId)
        {
            var payment = await _paymentRentalRequestRepository
                .GetAll()
                .FirstOrDefaultAsync(x=>x.Id == paymentId);

            if (payment == null)
            {
                return;
            }

            payment.Status = PaymentStatusDb.Canceled;
            await _paymentRentalRequestRepository.Update(payment);
        }

        public async Task MarkRefundAsSucceeded(string paymentId)
        {
            var payment = await _paymentRentalRequestRepository
                .GetAll()
                .Include(p=>p.RentalRequest)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null || payment.RentalRequest == null)
            {
                return;
            }

            payment.RentalRequest.PaymentActiveId = "";
            payment.RentalRequest.ApprovalStatus = ApprovalStatus.Rejected;
            payment.RentalRequest.DataChangeStatus = DateTime.UtcNow;

            await _rentalRequestRepository.Update(payment.RentalRequest);

            payment.Status = PaymentStatusDb.Refund;

            await _paymentRentalRequestRepository.Update(payment);
        }
    }
}
