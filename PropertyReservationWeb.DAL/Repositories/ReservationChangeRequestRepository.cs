using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class ReservationChangeRequestRepository : IBaseRepository<ReservationChangeRequest>
    {
        private readonly ApplicationDbContext _dbContext;
        public ReservationChangeRequestRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ReservationChangeRequest> GetAll()
        {
            return _dbContext.ReservationChangeRequests;
        }

        public async Task Delete(ReservationChangeRequest entity)
        {
            _dbContext.ReservationChangeRequests.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(ReservationChangeRequest entity)
        {
            await _dbContext.ReservationChangeRequests.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateRange(List<ReservationChangeRequest> entity)
        {
            await _dbContext.ReservationChangeRequests.AddRangeAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ReservationChangeRequest> Update(ReservationChangeRequest entity)
        {
            _dbContext.ReservationChangeRequests.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<ReservationChangeRequest>> UpdateRange(List<ReservationChangeRequest> entity)
        {
            _dbContext.ReservationChangeRequests.UpdateRange(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
