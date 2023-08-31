using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICarRepository Cars { get; }
        ICustomerRepositry Customers { get; }
        IDriverRepository Drivers { get; }
        IRentalRepository Rentals { get; }

        int Save();
    }

}
