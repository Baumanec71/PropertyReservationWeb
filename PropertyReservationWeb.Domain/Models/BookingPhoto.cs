namespace PropertyReservationWeb.Domain.Models
{
    public class BookingPhoto
    {
        public BookingPhoto() { }
        public long Id { get; set; }
        public long IdRentalRequest { get; set; }
        public byte[] ValuePhoto { get; set; } = new byte[0];
        public bool Before { get; set; }
        public bool DeleteStatus { get; set; }
        public DateTime DateCreate { get; set; }
        public RentalRequest RentalRequest { get; set; } = null!;
    }
}
