using PropertyReservationWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyReservationWeb.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task Delete(User entity);
        Task Create(User entity);
        IQueryable<User> GetAll();
        Task<User> Update(User entity);
    }
}
