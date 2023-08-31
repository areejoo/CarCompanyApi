using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;

namespace Web.Core.Interfaces
{
    public interface IRentalService
    {
        IQueryable<Rental> GetrentalsQueryable();
        Task<Rental> GetRentalByIdAsync(Guid id);
        Task<bool> AddRentalAsync(Rental entity);
        Task<bool> UpdateRentalAsync(Rental entity);
        Task<bool> DeleteRentalAsync(Guid id);
    }
}
