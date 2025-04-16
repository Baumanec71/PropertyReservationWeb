using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class PhotoRepository : IBaseRepository<Photo> , IPhotoRepository<Photo>
    {
        private readonly ApplicationDbContext _dbContext;
        public PhotoRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Photo> GetAll()
        {
            return _dbContext.Photos;
        }

        public async Task Delete(Photo entity)
        {
            _dbContext.Photos.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(Photo entity)
        {
            await _dbContext.Photos.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateRange(List<Photo> entity)
        {
            await _dbContext.Photos.AddRangeAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Photo> Update(Photo entity)
        {
            _dbContext.Photos.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<Photo>> UpdateRange(List<Photo> entity)
        {
            _dbContext.Photos.UpdateRange(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
