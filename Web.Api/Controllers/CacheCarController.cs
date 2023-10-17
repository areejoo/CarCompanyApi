using AutoMapper;
using Azure.Core;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Web.Api.Dtos.Incomming;
using Web.Api.Dtos.Outcomming;
using Web.Core.Entities;
using Web.Core.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class CacheCarController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICarService _carService;
        private readonly IMapper _mapper;

        public CacheCarController(IMemoryCache memoryCache, ICarService carService, IMapper mapper)
        {
            _memoryCache = memoryCache;
            _carService = carService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<CarListDto> GetListAsync([FromQuery] CarRequestDto request)
        {
            var cacheKey = "carList";
            var count = 0;
            IQueryable<Car> result = null;
            List<Car> result1 = null;
            result1 = (List<Car>)_memoryCache.Get(cacheKey);
            //output
            count = result1.Count();
            var resultDto = _mapper.Map<List<CarDto>>(result1);
            CarListDto carListDto = new CarListDto() { CarsPaginationList = resultDto, Count = count };

            return carListDto;
        }

        private IQueryable<Car> FilterCar(IQueryable<Car> query, CarRequestDto request)
        {
            query = query.Where(c => c.Number.ToString().Equals(request.Search)
                                           || c.Type.Equals(request.Search)
                                           || c.Color.Equals(request.Search)
                                           || c.WithDriver.ToString().Equals(request.Search)
                                           || c.DailyFare.ToString().Equals(request.Search)
                                           || c.EngineCapacity.ToString().Equals(request.Search));
            return query;
        }

        private IQueryable<Car> CreatePagination(IQueryable<Car> query, CarRequestDto request)
        {
            query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
            return query;
        }

        private IQueryable<Car> SortCar(IQueryable<Car> query, CarRequestDto request)
        {

            switch (request.Sort)
            {
                case "Color_desc":
                    query = query.OrderByDescending(c => c.Color);
                    break;
                case "EngineCapacity_desc":
                    query = query.OrderByDescending(c => c.EngineCapacity);
                    break;
                case "CapacityEngine_asc":
                    query = query.OrderBy(c => c.Type);
                    break;
                case "Type_desc":
                    query = query.OrderByDescending(c => c.Type);
                    break;
                case "Type_asc":
                    query = query.OrderBy(c => c.Type);
                    break;
                case "WithDriver_desc":
                    query = query.OrderByDescending(c => c.WithDriver);
                    break;
                case "WithDriver_asc":
                    query = query.OrderBy(c => c.WithDriver);
                    break;
                case "DailyFare_desc":
                    query = query.OrderByDescending(c => c.DailyFare);
                    break;
                case "DailyFare_asc":
                    query = query.OrderBy(c => c.DailyFare);
                    break;


                default:
                    query = query.OrderBy(c => c.Color);
                    break;

            }
            return query;
        }
    }
}