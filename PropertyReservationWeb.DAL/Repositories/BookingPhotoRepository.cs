using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Repositories
{
    public class BookingPhotoRepository : IBaseRepository<BookingPhoto>, IPhotoRepository<BookingPhoto>
    {
        private readonly ApplicationDbContext _dbContext;
        public BookingPhotoRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<BookingPhoto> GetAll()
        {
            return _dbContext.BookingPhotos;
        }

        public async Task Delete(BookingPhoto entity)
        {
            _dbContext.BookingPhotos.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(BookingPhoto entity)
        {
            await _dbContext.BookingPhotos.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateRange(List<BookingPhoto> entity)
        {
            await _dbContext.BookingPhotos.AddRangeAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<BookingPhoto> Update(BookingPhoto entity)
        {
            _dbContext.BookingPhotos.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<BookingPhoto>> UpdateRange(List<BookingPhoto> entity)
        {
            _dbContext.BookingPhotos.UpdateRange(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
