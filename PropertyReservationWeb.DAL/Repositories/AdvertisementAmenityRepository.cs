using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class AdvertisementAmenityRepository : IBaseRepository<AdvertisementAmenity>, IAdvertisementAmenityRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AdvertisementAmenityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<AdvertisementAmenity> GetAll()
        {
            return _dbContext.AdvertisementAmenities;
        }

        public async Task Delete(AdvertisementAmenity entity)
        {
            _dbContext.AdvertisementAmenities.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(AdvertisementAmenity entity)
        {
            await _dbContext.AdvertisementAmenities.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AdvertisementAmenity> Update(AdvertisementAmenity entity)
        {
            _dbContext.AdvertisementAmenities.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task CreateRange(List<AdvertisementAmenity> entity)
        {
            await _dbContext.AdvertisementAmenities.AddRangeAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<AdvertisementAmenity>> UpdateRange(List<AdvertisementAmenity> entity)
        {
            _dbContext.AdvertisementAmenities.UpdateRange(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
