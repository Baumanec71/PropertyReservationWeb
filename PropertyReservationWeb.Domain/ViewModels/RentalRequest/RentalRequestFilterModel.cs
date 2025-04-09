using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.ViewModels.RentalRequest
{
    public class RentalRequestFilterModel
    {
        public ApprovalStatus? SelectedApprovalStatus { get; set; }
        public bool? SelectedDeleteStatus { get; set; }
        public DateTime? SelectedBookingStartDate { get; set; }
        public DateTime? SelectedBookingFinishDate { get; set; }
        public DateTime? SelectedDataChangeStatus { get; set; }
        public long? SelectedIdAuthorRentalRequest { get; set; }
        public long? SelectedIdAuthorNeedAdvertisement { get; set; }
        public long? SelectedIdNeedAdvertisement { get; set; }
        public string? SelectedPaymentId { get; set; }
        public long? SelectedIdAuthor { get; set; }
        public List<ApprovalStatusOptionViewModel> types { get; set; } = new();
    }
}
