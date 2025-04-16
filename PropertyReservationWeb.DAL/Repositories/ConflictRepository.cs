using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class ConflictRepository : IBaseRepository<Conflict>
    {
        private readonly ApplicationDbContext _dbContext;
        public ConflictRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Conflict> GetAll()
        {
            return _dbContext.Conflicts;
        }

        public async Task Delete(Conflict entity)
        {
            _dbContext.Conflicts.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(Conflict entity)
        {
            await _dbContext.Conflicts.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Conflict> Update(Conflict entity)
        {
            _dbContext.Conflicts.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}