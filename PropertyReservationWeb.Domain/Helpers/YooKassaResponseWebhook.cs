using System.Text.Json;
using System.Text.Json.Serialization;

namespace PropertyReservationWeb.Domain.Helpers
{
        public class YooKassaResponseWebhook
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("event")]
            public string Event { get; set; } = string.Empty;

            [JsonPropertyName("object")]
            public JsonElement Object { get; set; }

            public object? GetObject()
            {
                return Event switch
                {
                    "payment.succeeded" => Object.Deserialize<PaymentSucceededObject>(),
                    "refund.succeeded" => Object.Deserialize<RefundSucceededObject>(),
                    _ => null
                };
            }
        }

        // Модель для события payment.succeeded
        public class PaymentSucceededObject
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("status")]
            public string Status { get; set; } = string.Empty;

            [JsonPropertyName("amount")]
            public Amount Amount { get; set; } = new();

            [JsonPropertyName("income_amount")]
            public Amount IncomeAmount { get; set; } = new();

            [JsonPropertyName("description")]
            public string Description { get; set; } = string.Empty;

            [JsonPropertyName("recipient")]
            public Recipient Recipient { get; set; } = new();

            [JsonPropertyName("payment_method")]
            public PaymentMethod PaymentMethod { get; set; } = new();

            [JsonPropertyName("captured_at")]
            public DateTime CapturedAt { get; set; }

            [JsonPropertyName("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonPropertyName("test")]
            public bool Test { get; set; }

            [JsonPropertyName("refunded_amount")]
            public Amount RefundedAmount { get; set; } = new();

            [JsonPropertyName("paid")]
            public bool Paid { get; set; }

            [JsonPropertyName("refundable")]
            public bool Refundable { get; set; }

            [JsonPropertyName("metadata")]
            public Dictionary<string, string>? Metadata { get; set; }

            [JsonPropertyName("authorization_details")]
            public AuthorizationDetails AuthorizationDetails { get; set; } = new();
        }

        // Модель для события refund.succeeded
        public class RefundSucceededObject
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("payment_id")]
            public string PaymentId { get; set; } = string.Empty;

            [JsonPropertyName("status")]
            public string Status { get; set; } = string.Empty;

            [JsonPropertyName("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonPropertyName("amount")]
            public Amount Amount { get; set; } = new();
        }

        // Общие классы
        public class Amount
        {
            [JsonPropertyName("value")]
            public string Value { get; set; } = string.Empty;

            [JsonPropertyName("currency")]
            public string Currency { get; set; } = string.Empty;
        }

        public class Recipient
        {
            [JsonPropertyName("account_id")]
            public string AccountId { get; set; } = string.Empty;

            [JsonPropertyName("gateway_id")]
            public string GatewayId { get; set; } = string.Empty;
        }

        public class PaymentMethod
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("saved")]
            public bool Saved { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; } = string.Empty;

            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;

            [JsonPropertyName("card")]
            public Card Card { get; set; } = new();
        }

        public class Card
        {
            [JsonPropertyName("first6")]
            public string First6 { get; set; } = string.Empty;

            [JsonPropertyName("last4")]
            public string Last4 { get; set; } = string.Empty;

            [JsonPropertyName("expiry_year")]
            public string ExpiryYear { get; set; } = string.Empty;

            [JsonPropertyName("expiry_month")]
            public string ExpiryMonth { get; set; } = string.Empty;

            [JsonPropertyName("card_type")]
            public string CardType { get; set; } = string.Empty;

            [JsonPropertyName("card_product")]
            public CardProduct CardProduct { get; set; } = new();

            [JsonPropertyName("issuer_country")]
            public string IssuerCountry { get; set; } = string.Empty;
        }

        public class CardProduct
        {
            [JsonPropertyName("code")]
            public string Code { get; set; } = string.Empty;
        }

        public class AuthorizationDetails
        {
            [JsonPropertyName("rrn")]
            public string Rrn { get; set; } = string.Empty;

            [JsonPropertyName("auth_code")]
            public string AuthCode { get; set; } = string.Empty;

            [JsonPropertyName("three_d_secure")]
            public ThreeDSecure ThreeDSecure { get; set; } = new();
        }

        public class ThreeDSecure
        {
            [JsonPropertyName("applied")]
            public bool Applied { get; set; }

            [JsonPropertyName("method_completed")]
            public bool MethodCompleted { get; set; }

            [JsonPropertyName("challenge_completed")]
            public bool ChallengeCompleted { get; set; }
        }
    
}

