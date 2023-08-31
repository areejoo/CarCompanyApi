using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Interfaces;

namespace Web.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyAppDbContext _dbContext;
        public ICarRepository Cars { get; }
        public ICustomerRepositry Customers { get; }
        public IDriverRepository Drivers { get; }
        public IRentalRepository Rentals { get; }


        public UnitOfWork(MyAppDbContext dbContext,
                            ICarRepository carRepo, ICustomerRepositry customerRepo, IDriverRepository driverRepository, IRentalRepository rentalRepository)
        {
            _dbContext = dbContext;
            Cars = carRepo;
            Customers = customerRepo;
            Drivers = driverRepository;
            Rentals = rentalRepository;
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
    }
}
