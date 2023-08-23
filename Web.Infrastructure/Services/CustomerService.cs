using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;
using Web.Core.Interfaces;

namespace Web.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        public IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddCustomerAsync(Customer entity)
        {
            if (entity != null)
            {
                await _unitOfWork.Customers.AddAsync(entity);
                var result = _unitOfWork.Save();

                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            return false;

        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            if (id !=null){
               await _unitOfWork.Customers.DeleteAsync(id);
                var result =   _unitOfWork.Save();
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public  async Task<Customer> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer != null)
            {
                return customer;
            }
            return null;
        }

        public IQueryable<Customer> GetCustomersQueryable()
        {
            var customerDetailsList = _unitOfWork.Customers.GetQueryable();
            return customerDetailsList;

        }

        public async Task<bool> UpdateCustomerAsync(Customer entity)
        {
            if (entity != null)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(entity.Id);
                if (customer != null)
                {
                    await _unitOfWork.Customers.UpdateAsync(entity);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
    
    }
}
