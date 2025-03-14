using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class AmenityRepository : IBaseRepository<Amenity>
    {
        private readonly ApplicationDbContext _dbContext;
        public AmenityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Amenity> GetAll()
        {
            return _dbContext.Amenities;
        }

        public async Task Delete(Amenity entity)
        {
            _dbContext.Amenities.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(Amenity entity)
        {
            await _dbContext.Amenities.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Amenity> Update(Amenity entity)
        {
            _dbContext.Amenities.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
