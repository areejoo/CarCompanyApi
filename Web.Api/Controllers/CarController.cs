using Microsoft.AspNetCore.Mvc;
using Web.Core.Entities;
using Web.Core.Interfaces;
using Web.Api.Dtos.Incomming;
using Web.Api.Dtos.Outcomming;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using web.api.Dtos.Incomming;
using Web.Api.Dtos.Incomming.DriverDto;
using System.Collections;

namespace web.Api.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class CarController : Controller
    {

        private readonly IMapper _mapper;
        public readonly ICarService _carService;

        public CarController(IMapper mapper, ICarService cartService)
        {
            _mapper = mapper;
            _carService = cartService;

        }



        [HttpGet]
        [Consumes("application/json")]
        public async Task<CarListDto> GetListAsync([FromQuery] CarRequestDto request)
        {
            var query = _carService.GetCarsQueryable();
            //filtering
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = FilterCar(query, request);
            }
            //get Count
            var count = await query.CountAsync();
            //apply  sorting
            if (!string.IsNullOrEmpty(request.Sort))
            {

                query = SortCar(query, request);
            }

            //apply pagination

            query = CreatePagination(query, request);

            //output
            var result = await query.ToListAsync();
            var resultDto = _mapper.Map<List<CarDto>>(result);
            CarListDto carListDto = new CarListDto() { CarsPaginationList = resultDto, Count = count };


            return carListDto;
        }

        [HttpGet("{id}")]
        public async Task<CarDto> GetAsync(Guid id)
        {
            var car = await _carService.GetCarByIdAsync(id);

            var carDto = _mapper.Map<CarDto>(car);

            return carDto;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCarDto createCarDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            CarDto carDto = null;
            IQueryable<Car> query = null;
         
            try
            {
                if (createCarDto.Number<=0)
                {
                    return BadRequest("number of car is  required");

                }
                query = _carService.GetCarsQueryable();
                query = query.Where(c => c.Number == createCarDto.Number);


                if (query.Count() == 0)
                {
                    var carEntity = _mapper.Map<Car>(createCarDto);
                    var result1 = await _carService.AddCarAsync(carEntity);
                    if (result1)
                        carDto = _mapper.Map<CarDto>(carEntity);
                }
                else
                {
                    return BadRequest("number is reserved by another car");
                }

            }
            catch (Exception e)
            {

            }
            return Ok("Done");


        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAsync([FromBody] UpdateCarDto updateCarDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            if (updateCarDto.Id == null || updateCarDto.Id==Guid.Empty)
                return BadRequest("id is required");


            var carEntity = await _carService.GetCarByIdAsync(updateCarDto.Id);
            IQueryable<Car> query = null;
            bool result1 = false;

            try
            {
                if (carEntity != null)
                {
                    if (updateCarDto.Number != null)
                    {
                        query = _carService.GetCarsQueryable();
                        query = query.Where(c => c.Number == updateCarDto.Number);
                        //car with this number isnt found
                        if (query.Count() == 0)
                        {
                            carEntity.Number = (int)(updateCarDto.Number);
                        }
                        else
                        {
                            return BadRequest("number of car is reserved by another car");
                        }
                    }
                    carEntity.DriverId = updateCarDto.DriverId != null ? updateCarDto.DriverId : carEntity.DriverId;
                    carEntity.Color = updateCarDto.Color != null ? updateCarDto.Color : carEntity.Color;
                    carEntity.Type = updateCarDto.Type != null ? updateCarDto.Type : carEntity.Type;
                    carEntity.WithDriver = updateCarDto.DriverId != null ? true : false;
                    carEntity.IsAvailable = (bool)(updateCarDto.IsAvailabe != null ? updateCarDto.IsAvailabe : carEntity.IsAvailable);

                    //carEntity = _mapper.Map<Car>(updateCarDto);
                    result1 = await _carService.UpdateCarAsync(carEntity);
                    if (result1)
                        return Ok("done");
                    else
                        return BadRequest("error in update");

                }

                else
                {
                    return BadRequest("car is not found");
                }

            }
            catch (Exception e)
            {
                //_logger.LogInformation(e.ToString());
            }

            return Ok();



        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = false;
            try
            {
                result = await _carService.DeleteCarAsync(id);

            }

            catch (Exception e)
            {

                return BadRequest();
            }
            if (result)
                return Ok();
            else
                return NotFound();


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
