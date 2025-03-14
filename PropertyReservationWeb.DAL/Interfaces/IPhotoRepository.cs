using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Interfaces
{
    public interface IPhotoRepository
    {
        Task CreateRange(List<Photo> entity);
    }
}
