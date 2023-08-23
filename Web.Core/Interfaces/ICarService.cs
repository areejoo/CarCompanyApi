using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;

namespace Web.Core.Interfaces
{
    public interface ICarService
    {
        IQueryable<Car> GetCarsQueryable();
        Task<Car> GetCarByIdAsync(Guid id);
        Task <bool> AddCarAsync(Car entity);
        Task <bool> UpdateCarAsync(Car entity);
        Task <bool> DeleteCarAsync(Guid id);
    }
}
