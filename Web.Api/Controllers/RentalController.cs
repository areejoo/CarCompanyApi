using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Api.Dtos.Incomming;
using Web.Api.Dtos.Incomming.CustomerDto;
using Web.Api.Dtos.Incomming.RentalDto;
using Web.Api.Dtos.Outcomming;
using Web.Api.Dtos.Outcomming.CustomerDto;
using Web.Api.Dtos.Outcomming.RentalDto;
using Web.Core.Entities;
using Web.Core.Interfaces;
using Web.Infrastructure.Services;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : Controller
    {
        private readonly IRentalService _rentalService;
        private readonly IDriverService _driverService;
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;

        private readonly IMapper _mapper;

        public RentalController(ICustomerService customerService, IRentalService rentalService, IMapper mapper, ICarService carService, IDriverService driverService)
        {
            _rentalService = rentalService;
            _mapper = mapper;
            _carService = carService;
            _driverService = driverService;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<RentalListDto> GetListAsync([FromQuery] RentalRequestDto request)
        {
            var query = _rentalService.GetrentalsQueryable();
            //filtering
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = FilterRental(query, request);
            }
            //get Count
            var count = await query.CountAsync();
            //apply  sorting
            if (!string.IsNullOrEmpty(request.Sort))
            {

                query = SortRental(query, request);
            }

            //apply pagination

            query = CreatePagination(query, request);

            //output
            var result = await query.ToListAsync();
            var resultDto = _mapper.Map<List<RentalDto>>(result);
            RentalListDto carListDto = new RentalListDto() { RentalsPaginationList = resultDto, Count = count };


            return carListDto;
        }



        [HttpGet("{id}")]
        public async Task<RentalDto> GetAsync(Guid id)
        {
            var rental = await _rentalService.GetRentalByIdAsync(id);

            var rentalDto = _mapper.Map<RentalDto>(rental);

            return rentalDto;
        }

        [HttpPost]
        //without returen replacment driver
        public async Task<IActionResult> CreateAsync([FromBody] CreateRentalDto createRentalDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            Rental rentalEntity = _mapper.Map<Rental>(createRentalDto);

            if (createRentalDto.CarId != Guid.Empty)
            {
                var resultCarFound = await CheckCarAvaiableAsync(createRentalDto.CarId);
                if (resultCarFound == 0)   //car not found
                    return BadRequest("car not found");
                else //car found
                {
                    var result = CheckCarAvaiableByDateAsync(createRentalDto.CarId, createRentalDto.StartDate, createRentalDto.RentTerm);
                    if (result == 0)
                        return BadRequest("car is not availble in this date1");
                }
            }

            if (createRentalDto.CustomerId != Guid.Empty)
            {
                var resultCustomerFound = await CheckCustomerAvaiableAsync(createRentalDto.CustomerId);
                if (resultCustomerFound == 0)
                    return BadRequest("customer not found");
            }

            if (createRentalDto.DriverId != Guid.Empty)

            {
                var resultDriverFound = await CheckDriverFoundAsync((Guid)createRentalDto.DriverId);
                if (resultDriverFound == 0)//driver not found
                {
                    return BadRequest("Driver not found");

                }
                else if (resultDriverFound == 2)//driver not available
                {
                    var replacmentDriver = await FindReplacmentDriver((Guid)createRentalDto.DriverId);
                    if (replacmentDriver == null)
                    {
                        return BadRequest("no driver is availble in company ");
                    }
                    rentalEntity.DriverId = replacmentDriver.Id;
                    replacmentDriver.IsAvailable = false;
                    await _driverService.UpdateDriverAsync(replacmentDriver);

                }
                else // Driver available
                {
                    var driver = await _driverService.GetDriverByIdAsync((Guid)createRentalDto.DriverId);
                    driver.IsAvailable = false;
                    await _driverService.UpdateDriverAsync(driver);
                }

            }//driver


            if (createRentalDto.Total == 0) {
                var carRental = await _carService.GetCarByIdAsync(createRentalDto.CarId);
                var total = createRentalDto.RentTerm * carRental.DailyFare;
                rentalEntity.Total = total;
            
            }
            var isAdded = await _rentalService.AddRentalAsync(rentalEntity);
            if (isAdded)
                return Ok("done");

            return BadRequest("error in added");

        }

        private int CheckCarAvaiableByDateAsync(Guid carId, DateTime startDate, int rentTerm)
        {
            int result = 0;
            var rentals = _rentalService.GetrentalsQueryable();
            rentals = rentals.Where(c => c.CarId == carId);
            if (rentals != null)//car in rental table

                rentals = rentals.Where(c => c.StartDate.AddDays(c.RentTerm) >= startDate &&
                c.StartDate <= startDate.AddDays(rentTerm));

            if (rentals.Count() == 0)
                result = 1;//car available in this date

            return result;
        }
        [HttpPut("{id}")]
        ///////////////////////////////without returen replacment driver
        public async Task<IActionResult>
            UpdateAsync([FromBody] UpdateRenatlDto updateRenatlDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var rentalEntity = await _rentalService.GetRentalByIdAsync(updateRenatlDto.Id);
            if (rentalEntity == null)
                return BadRequest("This rental is not found ");
            if (updateRenatlDto.CarId != null)
            {
                var resultCarFound = await CheckCarAvaiableAsync((Guid)updateRenatlDto.CarId);
                if (resultCarFound == 0)
                    return BadRequest("car not found");
                else
                {
                    int result = 0;
                    if (updateRenatlDto.StartDate != null)
                    {
                        result = CheckCarAvaiableByDateAsync((Guid)updateRenatlDto.CarId, (DateTime)updateRenatlDto.StartDate, (int)updateRenatlDto.RentTerm);
                    }
                    else
                    {
                        result = CheckCarAvaiableByDateAsync((Guid)updateRenatlDto.CarId, rentalEntity.StartDate, rentalEntity.RentTerm);
                    }
                    if (result == 0)
                        return BadRequest("car is not availble in this date1");
                }
                rentalEntity.CarId = (Guid)updateRenatlDto.CarId;
                rentalEntity.StartDate = (DateTime)updateRenatlDto.StartDate;
            }

            if (updateRenatlDto.CustomerId != null)
            {
                var resultCustomerFound = await CheckCustomerAvaiableAsync((Guid)updateRenatlDto.CustomerId);
                if (resultCustomerFound == 0)
                    return BadRequest("customer not found");
                rentalEntity.CustomerId = (Guid)updateRenatlDto.CustomerId;
            }

            if (updateRenatlDto.DriverId != null)

            {
                var resultDriverFound = await CheckDriverFoundAsync((Guid)updateRenatlDto.DriverId);
                if (resultDriverFound == 0)
                {
                    return BadRequest("Driver not found");

                }
                else if (resultDriverFound == 2)//driver not available
                {
                    var replacmentDriver = await FindReplacmentDriver((Guid)updateRenatlDto.DriverId);
                    if (replacmentDriver == null)
                    {
                        return BadRequest("no driver is availble in company ");
                    }
                }
                else
                {
                    //driver availble
                    var oldDriver = await _driverService.GetDriverByIdAsync((Guid)rentalEntity.DriverId);
                    oldDriver.IsAvailable = true;
                    await _driverService.UpdateDriverAsync(oldDriver);


                    var driver = await _driverService.GetDriverByIdAsync((Guid)updateRenatlDto.DriverId);
                    driver.IsAvailable = false;
                    await _driverService.UpdateDriverAsync(driver);

                    rentalEntity.DriverId = driver.Id;
                }

            }//driver

            if (updateRenatlDto.StartDate != null)
            {
                int result = 0;
                if (updateRenatlDto.RentTerm != null)//found in request
                    result = CheckCarAvaiableByDateAsync((Guid)rentalEntity.CarId, (DateTime)updateRenatlDto.StartDate, (int)updateRenatlDto.RentTerm);
                else
                    result = CheckCarAvaiableByDateAsync(rentalEntity.CarId, (DateTime)updateRenatlDto.StartDate, rentalEntity.RentTerm);

                if (result == 1)
                    rentalEntity.StartDate = (DateTime)updateRenatlDto.StartDate;
                else
                    return BadRequest("this date is not available ");

            }

            rentalEntity.RentTerm = (int)(updateRenatlDto.RentTerm != null ? updateRenatlDto.RentTerm : rentalEntity.RentTerm);

            rentalEntity.Status = (Core.Enums.RentalStatus)((updateRenatlDto.Status != null) ? updateRenatlDto.Status : rentalEntity.Status);
            var isUpdated = await _rentalService.UpdateRentalAsync(rentalEntity);
            if (isUpdated)
                return Ok("done");

            return BadRequest("error in updated");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = false;
            try
            {
                result = await _rentalService.DeleteRentalAsync(id);

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

        private async Task<int> CheckDriverFoundAsync(Guid driverId)
        {
            int result = 0;
            var driverById = await _driverService.GetDriverByIdAsync(driverId);
            if (driverById != null)//driver is found
            {
                if (driverById.IsAvailable == true)//driver is available
                {
                    result = 1;//availabe
                    return result;
                }
                else
                {
                    result = 2;//not availabe

                }
            }
            return result;//not found
        }

        private async Task<Driver> FindReplacmentDriver(Guid driverId)
        {
            Driver driver = null;
            var driverById = await _driverService.GetDriverByIdAsync(driverId);
            var replacmentDriver = await _driverService.GetDriverByIdAsync((Guid)driverById.ReplacementDriverId);
            if (replacmentDriver.IsAvailable == true)//replacment`Driver is available
            {
                driver = replacmentDriver;
            }

            else
            {
                driver = replacmentDriver.ReplacementDriverId != null ? await FindReplacmentDriver((Guid)replacmentDriver.ReplacementDriverId) : null;

            }

            if (driver != null)
            {
                driver.IsAvailable = false;
                await _driverService.UpdateDriverAsync(driver);
            }
            return driver;
        }

        private async Task<int> CheckCustomerAvaiableAsync(Guid customerId)
        {
            int result = 0;
            var customer = await _customerService.GetCustomerByIdAsync(customerId);


            if (customer != null)
                result = 1;

            return result;

        }

        private async Task<int> CheckCarAvaiableAsync(Guid carId)
        {
            int result = 0;
            var car = await _carService.GetCarByIdAsync(carId);
            if (car != null)
            {
                result = 1;
            }
            return result;


        }

        private IQueryable<Rental> CreatePagination(IQueryable<Rental> query, RentalRequestDto request)
        {
            query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
            return query;
        }

        private IQueryable<Rental> SortRental(IQueryable<Rental> query, RentalRequestDto request)
        {
            switch (request.Sort)
            {
                case "StartDate_asc":
                    query = query.OrderByDescending(c => c.StartDate);
                    break;
                case "RentTerm_asc":
                    query = query.OrderBy(c => c.RentTerm);
                    break;

                case "RentTerm_desc":
                    query = query.OrderByDescending(c => c.RentTerm);
                    break;
                case "Status_asc":
                    query = query.OrderBy(c => c.Status);
                    break;
                case "Status_desc":
                    query = query.OrderByDescending(c => c.Status);
                    break;


                default:
                    query = query.OrderBy(c => c.StartDate);
                    break;
            }
            return query;
        }

        private IQueryable<Rental> FilterRental(IQueryable<Rental> query, RentalRequestDto request)
        {
            query = query.Where(c => c.CustomerId.Equals(request.Search.Trim())
                                          || c.CarId.Equals(request.Search.Trim())
                                          || c.DriverId.ToString().Equals(request.Search)
                                          || c.Status.ToString().Equals(request.Search)
                                          || c.StartDate.ToString().Equals(request.Search)
                                          || c.RentTerm.ToString().Equals(request.Search)
                                          );

            return query;
        }
    }
}
