using Microsoft.EntityFrameworkCore;
using Web.Api.Middleware;
using Web.APi.Profilles;
using Web.Core.Interfaces;
using Web.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Web.Infrastructure.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

//added by me
builder.Services.AddDIServices(builder.Configuration);
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(CarProfile));

var app = builder.Build();
//added by me


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseMiddleware<ErrorWrappingMiddleware>();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
