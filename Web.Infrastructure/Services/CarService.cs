using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;
using Web.Core.Interfaces;

namespace Web.Infrastructure.Services
{
    public class CarService : ICarService
    {
        public IUnitOfWork _unitOfWork;

        public CarService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> AddCarAsync(Car entity)
        {
            if (entity != null)
            {
                await _unitOfWork.Cars.AddAsync(entity);
                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }


        public async Task<bool> DeleteCarAsync(Guid id)
        {
            if (id != null)
            {
                var car = await _unitOfWork.Cars.GetByIdAsync(id);

                if (car != null)
                {
                    await _unitOfWork.Cars.DeleteAsync(id);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }



        public async Task<Car> GetCarByIdAsync(Guid id)
        {
            var Car = await _unitOfWork.Cars.GetByIdAsync(id);
            if (Car != null)
            {
                return Car;
            }
            return null;
        }

        public IQueryable<Car> GetCarsQueryable()
        {
            var carDetailsList = _unitOfWork.Cars.GetQueryable();
            return carDetailsList;

        }

        public async Task<bool> UpdateCarAsync(Car entity)
        {
            if (entity != null)
            {
                var car = await _unitOfWork.Cars.GetByIdAsync(entity.Id);
                if (car != null)
                {
                    //car.Color = entity.Color;
                    //car.Type = entity.Type;
                    //car.WithDriver = entity.DriverId == null ? false : true;
                    //car.DailyFare = entity.DailyFare;
                    //car.Rentals = entity.Rentals;

                    await _unitOfWork.Cars.UpdateAsync(entity);

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
