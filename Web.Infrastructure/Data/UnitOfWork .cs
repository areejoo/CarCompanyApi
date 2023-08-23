using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Interfaces;

namespace Web.Infrastructure.Data
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly MyAppDbContext _dbContext;
        public ICarRepository Cars { get; }

        public UnitOfWork(MyAppDbContext dbContext,
                            ICarRepository carRepo)
        {
            _dbContext = dbContext;
            Cars = carRepo;
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
