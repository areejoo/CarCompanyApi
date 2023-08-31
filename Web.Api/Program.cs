using Microsoft.EntityFrameworkCore;
using Web.APi.Profilles;
using Web.Core.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
//added by me
builder.Services.AddDIServices(builder.Configuration);
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddControllers();
// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(CarProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
