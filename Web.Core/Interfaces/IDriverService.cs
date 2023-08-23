using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;

namespace Web.Core.Interfaces
{
    public interface IDriverService
    {

        IQueryable<Driver> GetDriversQueryable();
        Task<Driver> GetDriverByIdAsync(Guid id);
        Task<bool> AddDriverAsync(Driver entity);
        Task<bool> UpdateDriverAsync(Driver entity);
        Task<bool> DeleteDriverAsync(Guid id);
    }
}
