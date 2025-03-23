using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.Models
{
    public class User
    {
        public User() { }
        public long Id { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BonusPoints { get; set; } = 0;
        public decimal Balance { get; set; } = 0;
        public bool Status { get; set; }
        public double Rating { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public byte[]? Avatar { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public List<Advertisement> Advertisements { get; set; } = new();
        public List<RentalRequest> RentalRequests { get; set; } = new();
        public List<ConversationRoom> ConversationRooms1 { get; set; } = new();
        public List<ConversationRoom> ConversationRooms2 { get; set; } = new();
        public List<ApprovalRequest> ApprovalRequests { get; set; } = new();
    }
}
