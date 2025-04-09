using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Interfaces
{
    public interface IBonusTransactionRepository
    {
        Task CreateRange(List<BonusTransaction> entity);
        Task<List<BonusTransaction>> UpdateRange(List<BonusTransaction> entity);
    }
}
