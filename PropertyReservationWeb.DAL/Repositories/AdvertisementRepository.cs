using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class AdvertisementRepository : IBaseRepository<Advertisement>
    {
        private readonly ApplicationDbContext _dbContext;
        public AdvertisementRepository( ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Advertisement> GetAll()
        {
            return _dbContext.Advertisements;
        }

        public async Task Delete(Advertisement entity)
        {
            _dbContext.Advertisements.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(Advertisement entity)
        {
            await _dbContext.Advertisements.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Advertisement> Update(Advertisement entity)
        {
            _dbContext.Advertisements.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
