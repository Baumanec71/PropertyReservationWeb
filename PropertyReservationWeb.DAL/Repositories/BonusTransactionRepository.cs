using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class BonusTransactionRepository : IBaseRepository<BonusTransaction> , IBonusTransactionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BonusTransactionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(BonusTransaction entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<BonusTransaction> GetAll()
        {
            return _dbContext.BonusTransactions;
        }

        public async Task Delete(BonusTransaction entity)
        {
            _dbContext.BonusTransactions.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<BonusTransaction> Update(BonusTransaction entity)
        {
            _dbContext.BonusTransactions.Update(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task CreateRange(List<BonusTransaction> entity)
        {
            await _dbContext.BonusTransactions.AddRangeAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<BonusTransaction>> UpdateRange(List<BonusTransaction> entity)
        {
            _dbContext.BonusTransactions.UpdateRange(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
