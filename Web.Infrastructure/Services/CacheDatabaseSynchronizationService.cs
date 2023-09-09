using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Web.Core.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Services;

public class CacheDatabaseSynchronizationService : IHostedService
{
    private readonly IMemoryCache _cache;
    private readonly ICarService _carService;


    public CacheDatabaseSynchronizationService(IMemoryCache cache,ICarService carService)
    {
        _cache = cache;
        _carService = carService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {

        int i = 0;
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Cleanup or perform any necessary actions when the application is shutting down
        return Task.CompletedTask;
    }
}