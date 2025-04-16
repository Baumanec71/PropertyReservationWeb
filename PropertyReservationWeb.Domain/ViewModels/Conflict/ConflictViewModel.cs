namespace PropertyReservationWeb.Domain.ViewModels.Conflict
{
    public record ConflictViewModel(
     long Id,
     long RentalRequestId,
     long CreatedByUserId,
     long? ResolvedByAdminId,
     string Description,
     string Status,
     DateTime DateCreated,
     DateTime? DateResolved
    );
}
