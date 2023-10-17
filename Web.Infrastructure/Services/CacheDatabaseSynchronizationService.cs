using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Web.Core.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Services;

public class CacheDatabaseSynchronizationService : BackgroundService
{
    private readonly IMemoryCache _cache;
    private readonly ICarService _carService;
    private readonly IServiceScopeFactory _scopeFactory;



    public CacheDatabaseSynchronizationService(IMemoryCache cache,ICarService carService, IServiceScopeFactory scopeFactory)
    {
        _cache = cache;
        _carService = carService;
        _scopeFactory = scopeFactory;
    }
    public void DoWork()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var s = scope.ServiceProvider.GetRequiredService<CarService>();
            
        }
    }

  

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

   
}