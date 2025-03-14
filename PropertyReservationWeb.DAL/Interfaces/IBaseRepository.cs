namespace PropertyReservationWeb.DAL.Interfaces
{
    public interface IBaseRepository<T>
    {
        IQueryable<T> GetAll();
        Task Create(T entity);
        Task Delete(T entity);
        Task<T> Update(T entity);
    }
}
