using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core.Interfaces;
using Web.Infrastructure.Data;

namespace Web.Infrastructure.Services
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MyAppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();

            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICarRepository, CarRepository>();
            services.AddScoped<ICustomerRepositry, CustomerRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IRentalRepository, RenatalRepository>();
            //services.AddHostedService<CacheDatabaseSynchronizationService>();
            //services.AddScoped<IHostedService>(provider => provider.GetService<CacheDatabaseSynchronizationService>());
            //services.AddHostedService<CacheDatabaseSynchronizationService>();

            services.AddHostedService<ConsumeScopedServiceHostedService>();
            services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

            return services;
        }
    }
}
