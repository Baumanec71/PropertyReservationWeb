using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.Models
{
    public class ApprovalRequest
    {
        public long Id { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateChange { get; set; }
        public long IdUserAdmin { get; set; }
        public User User { get; set; } = new User();
        public long IdAdvertisement { get; set; }
        public Advertisement Advertisement { get; set; } = null!;
        public ApprovalStatus Status { get; set; }
    }
}
