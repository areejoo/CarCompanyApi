using Microsoft.EntityFrameworkCore;
using Web.APi.Profilles;
using Web.Core.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDIServices(builder.Configuration);
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddControllers();
// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddDbContext<MyAppDbContext>(options => options.UseSqlServer(
//                    builder.Configuration.GetConnectionString("DefaultConnection"),
//                    optionsBuilder => optionsBuilder.MigrationsAssembly("Web.Infrastructure")));



//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//builder.Services.AddScoped<ICarRepository, CarRepository>();



//builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
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
