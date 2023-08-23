using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;
using Web.Core.Interfaces;

namespace Web.Infrastructure.Services
{
    public class DriverService : IDriverService
    {
        public IUnitOfWork _unitOfWork;

        public DriverService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public  async Task<bool> AddDriverAsync(Driver entity)
        {

            if (entity != null)
            {
                await _unitOfWork.Drivers.AddAsync(entity);
                var result = _unitOfWork.Save();

                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            return false;


        }

        public async Task<bool> DeleteDriverAsync(Guid id)
        {
            if(id != null){
                await _unitOfWork.Drivers.DeleteAsync(id);
                var result = _unitOfWork.Save();
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public async  Task<Driver> GetDriverByIdAsync(Guid id)
        {
            var driver = await _unitOfWork.Drivers.GetByIdAsync(id);
            if (driver != null)
            {
                return driver;
            }
            return null;
        }

        public IQueryable<Driver> GetDriversQueryable()
        {
            var driverDetailsList = _unitOfWork.Drivers.GetQueryable();
            return driverDetailsList;
        }

        public async Task<bool> UpdateDriverAsync(Driver entity)
        {
            if (entity != null)
            {
                var driver = await _unitOfWork.Drivers.GetByIdAsync(entity.Id);
                if (driver != null)
                {
                    await _unitOfWork.Drivers.UpdateAsync(entity);

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
