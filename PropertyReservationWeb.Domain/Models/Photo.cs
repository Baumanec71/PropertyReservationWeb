namespace PropertyReservationWeb.Domain.Models
{
    public class Photo
    {
        public Photo() { }
        public long Id { get; set; }
        public long IdAdvertisement { get; set; }

        public byte[] ValuePhoto { get; set; } = new byte[0];
        public bool DeleteStatus {  get; set; }
        public Advertisement Advertisement { get; set; } = null!;
    }
}
