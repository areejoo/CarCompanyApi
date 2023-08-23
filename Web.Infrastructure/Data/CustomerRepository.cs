using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;
using Web.Core.Interfaces;


namespace Web.Infrastructure.Data
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepositry
    {
        public CustomerRepository(MyAppDbContext context) : base(context)
        {
        }
    }
}
