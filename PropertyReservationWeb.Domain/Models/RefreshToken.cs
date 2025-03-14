namespace PropertyReservationWeb.Domain.Models
{
    public class RefreshToken
    {
        public long Id { get; set; } // Идентификатор в БД
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string? CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;
        public long IdUser { get; set; }
        public User User { get; set; } = null!;
    }
}
