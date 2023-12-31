﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Web.Api.Dtos.Incomming.CustomerDto;
using Web.Api.Dtos.Incomming.DriverDto;
using Web.Api.Dtos.Outcomming.CustomerDto;
using Web.Api.Dtos.Outcomming.DriverDto;
using Web.Core.Entities;
using Web.Core.Interfaces;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly IMapper _mapper;

        public DriverController(IDriverService driverService, IMapper mapper)
        {
            _driverService = driverService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<DriverListDto> GetListAsync([FromQuery] DriverRequestDto request)
        {
            var query = _driverService.GetDriversQueryable();
            //filtering
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = FilterDriver(query, request);
            }
            //get Count
            var count = await query.CountAsync();
            //apply  sorting
            if (!string.IsNullOrEmpty(request.Sort))
            {

                query = SortDriver(query, request);
            }

            //apply pagination

            query = CreatePagination(query, request);

            //output
            var result = await query.ToListAsync();
            var resultDto = _mapper.Map<List<DriverDto>>(result);
            DriverListDto driverListDto = new DriverListDto() { DriverPaginationList = resultDto, Count = count };


            return driverListDto;
        }


        [HttpGet("{id}")]
        public async Task<DriverDto> GetAsync(Guid id)
        {
            var driver = await _driverService.GetDriverByIdAsync(id);

            var driverDto = _mapper.Map<DriverDto>(driver);

            return driverDto;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateDriverDto createDriverDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            DriverDto driverDto = null;
            IQueryable<Driver> query = null;

            try
            {
                if (createDriverDto.Name == null)
                {
                    return BadRequest("name of driver is requird");

                }
                if (createDriverDto.Phone != null)
                {
                    query = _driverService.GetDriversQueryable();
                    query = query.Where(c => c.Phone == createDriverDto.Phone);

                    if (query.Count() != 0)
                    {
                        return BadRequest("phone is reserved by another driver");
                    }
                }

                var driverEntity = _mapper.Map<Driver>(createDriverDto);
                var result1 = await _driverService.AddDriverAsync(driverEntity);
                if (result1)
                    return Ok("done");

            }
            catch (Exception e)
            {

            }
            return Ok();


        }


        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAsync([FromBody] UpdateDriverDto updateDriverDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            DriverDto driverDto = null;
            IQueryable<Driver> query = null;
            Driver driver = null;
            if (updateDriverDto.Id == null || updateDriverDto.Id == Guid.Empty)
                return BadRequest("id is required");


            bool result1 = false;

            try
            {
                driver = await _driverService.GetDriverByIdAsync(updateDriverDto.Id);

                if (driver != null)
                {

                    if (updateDriverDto.Phone != null)
                    {

                        query = query.Where(c => c.Phone == updateDriverDto.Phone);

                        //customer with this email isnt found
                        if (query.Count() != 0)
                        {
                            return BadRequest("phone number is used by another driver");

                        }
                    }
                        driver.Name = updateDriverDto != null ? updateDriverDto.Name : driver.Name;
                        driver.Phone = updateDriverDto != null ? updateDriverDto.Phone : driver.Phone;
                        driver.ReplacementDriverId = (updateDriverDto.ReplacementDriverId != null ? updateDriverDto.ReplacementDriverId : driver.ReplacementDriverId);
                        driver.IsAvailable = (bool)(updateDriverDto.IsAvailable != null ? updateDriverDto.IsAvailable : driver.IsAvailable);
                        result1 = await _driverService.UpdateDriverAsync(driver);
                        if (result1)
                            driverDto = _mapper.Map<DriverDto>(driver);

                    }
                    else

                        return BadRequest("driver is not found");

                }
            catch (Exception e)
            {
                //_logger.LogInformation(e.ToString());
            }



            return Ok("updated successfuly");
        }






        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = false;
            try
            {
                result = await _driverService.DeleteDriverAsync(id);

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
        private IQueryable<Driver> CreatePagination(IQueryable<Driver> query, DriverRequestDto request)
        {
            query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
            return query;
        }

        private IQueryable<Driver> SortDriver(IQueryable<Driver> query, DriverRequestDto request)
        {

            switch (request.Sort)
            {
                case "Name_desc":
                    query = query.OrderByDescending(c => c.Name);
                    break;
                case "Phone_desc":
                    query = query.OrderByDescending(c => c.Phone);
                    break;
                case "Phone_asc":
                    query = query.OrderBy(c => c.Phone);
                    break;
                case "IsAvailablle_desc":
                    query = query.OrderByDescending(c => c.IsAvailable);
                    break;
                case "IsAvalilabe_asc":
                    query = query.OrderBy(c => c.IsAvailable);
                    break;
                case "ReplacementDriverId_desc":
                    query = query.OrderByDescending(c => c.ReplacementDriverId);
                    break;
                case "ReplacementDriverId_asc":
                    query = query.OrderBy(c => c.ReplacementDriverId);
                    break;

                default:
                    query = query.OrderBy(c => c.Name);
                    break;

            }
            return query;
        }

        private IQueryable<Driver> FilterDriver(IQueryable<Driver> query, DriverRequestDto request)
        {
            query = query.Where(c => c.Name.Trim().Equals(request.Search)
                                           || c.Phone.Trim().Equals(request.Search)
                                           || c.IsAvailable.ToString().Equals(request.Search)
                                           || c.ReplacementDriverId.ToString().Trim().Equals(request.Search));

            return query;
        }

    }
}
