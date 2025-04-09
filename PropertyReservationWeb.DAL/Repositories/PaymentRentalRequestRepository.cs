using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class PaymentRentalRequestRepository : IBaseRepository<PaymentRentalRequest>
    {
        private readonly ApplicationDbContext _dbContext;
        public PaymentRentalRequestRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Create(PaymentRentalRequest entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(PaymentRentalRequest entity)
        {
            _dbContext.PaymentRentalRequests.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<PaymentRentalRequest> GetAll()
        {
            return _dbContext.PaymentRentalRequests;
        }

        public async Task<PaymentRentalRequest> Update(PaymentRentalRequest entity)
        {
            _dbContext.PaymentRentalRequests.Update(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
