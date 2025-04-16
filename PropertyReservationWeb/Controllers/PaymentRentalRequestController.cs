using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyReservationWeb.Domain.Helpers;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Service.Implementations;
using PropertyReservationWeb.Service.Interfaces;
using System;
using System.Security.Claims;
using System.Text.Json;

namespace PropertyReservationWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentRentalRequestController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentRentalRequestController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("GetPaymentRentalRequest")]
        public async Task<IActionResult> GetPaymentRentalRequest([FromQuery] string id)
        {
            var paymentResponse = await _paymentService.GetPaymentRentalRequest(id);
            if (paymentResponse.StatusCode != Domain.Enum.StatusCode.OK || paymentResponse.Data == null)
            {
                return BadRequest(paymentResponse.Description);
            }

            return Ok(new { paymentUrl = paymentResponse.Data.Url });
        }

        [Authorize]
        [HttpPost("CreateRefund")]
        public async Task<IActionResult> CreateRefund([FromQuery] string paymentId)
        {
            var idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _paymentService.CreateRefundAsync(paymentId, 0);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
                return Ok(response);
            
            return BadRequest(response.Description);
        }

        [HttpGet("GetYooKassaResponse")]
        public async Task<IActionResult> GetYooKassaResponse([FromQuery] string id)
        {
            var paymentResponse = await _paymentService.GetYooKassaResponse(id);

            if (string.IsNullOrEmpty(paymentResponse.ErrorMessage)) 
            {
                return BadRequest(paymentResponse.ErrorMessage);
            }

            return Ok(paymentResponse);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PaymentWebhook()
        {
            try
            {
                var content = await new StreamReader(Request.Body).ReadToEndAsync();
                Console.WriteLine($"Ответ сервера: {content}");

                var notification = JsonSerializer.Deserialize<YooKassaResponseWebhook>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (notification == null)
                {
                    Console.WriteLine("Неизвестный тип уведомления");
                    return BadRequest("Неизвестный тип уведомления");
                }

                var obj = notification.GetObject();
                if (obj == null)
                {
                    Console.WriteLine("Не удалось десериализовать объект вебхука");
                    return BadRequest("Не удалось десериализовать объект вебхука");
                }

                switch (notification.Event)
                {
                    case "payment.succeeded":
                        if (obj is PaymentSucceededObject payment && payment.Status == "succeeded")
                        {
                            Console.WriteLine($"Платеж {payment.Id} одобрен");
                            await _paymentService.MarkPaymentAsSucceeded(payment.Id);
                        }
                        break;

                    case "payment.canceled":
                        if (obj is PaymentSucceededObject canceledPayment && canceledPayment.Status == "canceled")
                        {
                            Console.WriteLine($"Платеж {canceledPayment.Id} отклонен");
                            await _paymentService.MarkPaymentAsCanceled(canceledPayment.Id);
                        }
                        break;

                    case "refund.succeeded":
                        if (obj is RefundSucceededObject refund && refund.Status == "succeeded")
                        {
                            Console.WriteLine($"Возврат {refund.Id} выполнен");
                            await _paymentService.MarkRefundAsSucceeded(refund.PaymentId);
                        }
                        break;

                    default:
                        Console.WriteLine($"Неизвестное событие: {notification.Event}");
                        return BadRequest($"Неизвестное событие: {notification.Event}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке вебхука: {ex}");
                return StatusCode(500, "Ошибка обработки уведомления");
            }
        }
    }
}
