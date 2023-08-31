using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;
using Web.Core.Interfaces;

namespace Web.Infrastructure.Services
{
    public class RentalService : IRentalService
    {
        public IUnitOfWork _unitOfWork;
        public RentalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public async  Task<bool> AddRentalAsync(Rental entity)
        {
            if (entity != null)
            {

                await _unitOfWork.Rentals.AddAsync(entity);
                var result = _unitOfWork.Save();

                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public  async Task<bool> DeleteRentalAsync(Guid id)
        {
            if (id != Guid.Empty)
            {
                await _unitOfWork.Rentals.DeleteAsync(id);
                var result = _unitOfWork.Save();
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public async  Task<Rental> GetRentalByIdAsync(Guid id)
        {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(id);
            if (rental != null)
            {
                return rental;
            }
            return null;
        }

        public IQueryable<Rental> GetrentalsQueryable()
        {
            var rentalDetailsList = _unitOfWork.Rentals.GetQueryable();
            return rentalDetailsList;
        }

        public  async Task<bool> UpdateRentalAsync(Rental entity)
        {
            if (entity != null)
            {
                var driver = await _unitOfWork.Rentals.GetByIdAsync(entity.Id);
                if (driver != null)
                {
                    await _unitOfWork.Rentals.UpdateAsync(entity);

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
