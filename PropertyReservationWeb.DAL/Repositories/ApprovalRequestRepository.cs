using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class ApprovalRequestRepository : IBaseRepository<ApprovalRequest>
    {
        private readonly ApplicationDbContext _dbContext;
        public ApprovalRequestRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ApprovalRequest> GetAll()
        {
            return _dbContext.ApprovalRequests;
        }

        public async Task Delete(ApprovalRequest entity)
        {
            _dbContext.ApprovalRequests.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(ApprovalRequest entity)
        {
            await _dbContext.ApprovalRequests.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ApprovalRequest> Update(ApprovalRequest entity)
        {
            _dbContext.ApprovalRequests.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
