using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;

namespace Web.Core.Interfaces
{
    public interface ICustomerService
    {
        IQueryable<Customer> GetCustomersQueryable();
        Task<Customer> GetCustomerByIdAsync(Guid id);
        Task<bool> AddCustomerAsync(Customer entity);
        Task<bool> UpdateCustomerAsync(Customer entity);
        Task<bool> DeleteCustomerAsync(Guid id);
    }
}
