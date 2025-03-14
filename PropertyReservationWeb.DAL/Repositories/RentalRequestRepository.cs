using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class RentalRequestRepository : IBaseRepository<RentalRequest>
    {
        private readonly ApplicationDbContext _dbContext;
        public RentalRequestRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(RentalRequest entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<RentalRequest> GetAll()
        {
            return _dbContext.RentalRequests;
        }

        public async Task Delete(RentalRequest entity)
        {
            _dbContext.RentalRequests.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RentalRequest> Update(RentalRequest entity)
        {
            _dbContext.RentalRequests.Update(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
