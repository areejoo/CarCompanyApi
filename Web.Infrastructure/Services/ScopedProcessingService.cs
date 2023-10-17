using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Interfaces;

namespace Web.Infrastructure.Services
{
    public class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ICarService _carService;
        private readonly IMemoryCache _cache;

        public ScopedProcessingService(ICarService carService, IMemoryCache cache)
        {
            _carService = carService;
            _cache = cache;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {


            var data =   _carService.GetCarsQueryable();

            // Load data into cache
            _cache.Set("carList", await data.ToListAsync(), TimeSpan.FromMinutes(30));
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    executionCount++;

            //    _logger.LogInformation(
            //        "Scoped Processing Service is working. Count: {Count}", executionCount);

            //    await Task.Delay(10000, stoppingToken);
            //}
        }
    }
}