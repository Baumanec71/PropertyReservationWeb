namespace PropertyReservationWeb.DAL.Interfaces
{
    public interface IPhotoRepository<T>
    {
        Task CreateRange(List<T> entity);
        Task<List<T>> UpdateRange(List<T> entity);
    }
}
