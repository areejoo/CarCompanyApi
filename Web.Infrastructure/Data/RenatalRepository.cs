using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Entities;
using Web.Core.Interfaces;

namespace Web.Infrastructure.Data
{
    public class RenatalRepository : GenericRepository<Rental>, IRentalRepository
    {
        public RenatalRepository(MyAppDbContext context) : base(context)
        {
        }
    }
}
