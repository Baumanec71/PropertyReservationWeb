using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class ReviewRepository : IBaseRepository<Review>
    {
        private readonly ApplicationDbContext _dbContext;
        public ReviewRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Review entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<Review> GetAll()
        {
            return _dbContext.Reviews;
        }

        public async Task Delete(Review entity)
        {
            _dbContext.Reviews.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Review> Update(Review entity)
        {
            _dbContext.Reviews.Update(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
