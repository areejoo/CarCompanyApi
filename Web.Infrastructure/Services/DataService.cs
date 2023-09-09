using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Interfaces;

namespace Web.Infrastructure.Services
{
    public class DataService 
    {
        private readonly IMemoryCache _cache;
        private readonly ICarService _carService;

        public DataService(IMemoryCache cache, ICarService carService)
        {
            _cache = cache;
            _carService = carService;
        }

        public void LoadDataIntoCache()
        {
            // Retrieve data from the database
            var data = _carService.GetCarsQueryable();

            // Load data into cache
            _cache.Set("carList", data, TimeSpan.FromMinutes(30)); // Adjust expiration time as needed
        }
    }
}
