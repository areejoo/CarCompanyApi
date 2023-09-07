using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Api.Dtos.Incomming.RentalDto;
using Web.Api.Dtos.Outcomming.RentalDto;
using Web.Core.Entities;
using Web.Core.Interfaces;
using Web.Api.Responces;

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

            foreach (var i in resultDto)
            {
                Driver driver = null;
                var customer = await _customerService.GetCustomerByIdAsync(i.CustomerId);
                i.CustomerName = customer.Name;
                var car = await _carService.GetCarByIdAsync(i.CarId);
                i.CarNumber = car.Number;

                if (i.DriverId.HasValue)
                {
                    driver = await _driverService.GetDriverByIdAsync((Guid)i.DriverId);
                    i.DriverName = driver.Name;

                }

            }
            RentalListDto carListDto = new RentalListDto() { RentalsPaginationList = resultDto, Count = count };


            return carListDto;
        }



        [HttpGet("{id}")]
        public async Task<RentalDto> GetAsync(Guid id)
        {
            var rental = await _rentalService.GetRentalByIdAsync(id);
            var rentalDto = _mapper.Map<RentalDto>(rental);

            Driver driver = null;
            var customer = await _customerService.GetCustomerByIdAsync(rental.CustomerId);
            rentalDto.CustomerName = customer.Name;

            var car = await _carService.GetCarByIdAsync(rentalDto.CarId);
            rentalDto.CarNumber = car.Number;

            if (rental.DriverId.HasValue)
            {
                driver = await _driverService.GetDriverByIdAsync((Guid)rental.DriverId);
                rentalDto.DriverName = driver.Name;

            }

            return rentalDto;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateRentalDto createRentalDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            Rental rentalEntity = _mapper.Map<Rental>(createRentalDto);
            DateTime startDate = DateTime.Now;
            if (createRentalDto.RentTerm <= 0)
            {
                return BadRequest(new ApiResponse(400, $"rent term must be bigger than zero {createRentalDto.RentTerm}"));

            }

            if (createRentalDto.StartDate.HasValue)
            {
                startDate = (DateTime)createRentalDto.StartDate;
            }

            rentalEntity.StartDate = startDate;// startDate is  cuurent date


            if (createRentalDto.CarId != Guid.Empty)
            {
                bool result = false;
                var resultCarFound = await CheckCarAvaiableAsync(createRentalDto.CarId);
                if (!resultCarFound)   //car not found
                    return NotFound(new ApiResponse(404, $"Car not found with id {createRentalDto.CarId}"));

                //car found
                result = CheckCarAvaiableByDateAsync(createRentalDto.CarId, startDate, createRentalDto.RentTerm, null);

                if (!result)
                    return BadRequest(new ApiResponse(400, $"car is not availble in this date {startDate}"));

            }

            else
            {
                return BadRequest(new ApiResponse(400, $"car is required"));
            }

            if (createRentalDto.CustomerId != Guid.Empty)
            {
                var resultCustomerFound = await CheckCustomerAvaiableAsync(createRentalDto.CustomerId);
                if (!resultCustomerFound)
                    return NotFound(new ApiResponse(404, $"Customer  not found with id {createRentalDto.CustomerId}"));
            }
            else
            {
                return BadRequest(new ApiResponse(400, $"Customer is required"));
            }


            if (createRentalDto.DriverId.HasValue)
            {
                Driver availableDriver = await getAvailbaleDriverAsync((Guid)createRentalDto.DriverId,startDate,createRentalDto.RentTerm);
                if (availableDriver == null)//driver not found or not availble
                {
                    return NotFound(new ApiResponse(404, $"driver not found or not availble with id  {createRentalDto.CarId}"));

                }

                rentalEntity.DriverId = availableDriver.Id;
                availableDriver.IsAvailable = false;
                await _driverService.UpdateDriverAsync(availableDriver);

            }//driver

            if (!createRentalDto.Total.HasValue)
            {
                var carRental = await _carService.GetCarByIdAsync(createRentalDto.CarId);
                var total = createRentalDto.RentTerm * carRental.DailyFare;
                rentalEntity.Total = total;

            }
            var isAdded = await _rentalService.AddRentalAsync(rentalEntity);
            if (isAdded)
                return Ok(new ApiResponse(200, $"added successfuly"));

            return BadRequest(new ApiResponse(500, $"error in??????????????????????? "));
        }

        private bool CheckCarAvaiableByDateAsync(Guid carId, DateTime startDate, int rentTerm, Guid? rental)
        {
            bool result = false;
            var rentals = _rentalService.GetrentalsQueryable();
            rentals = rentals.Where(c => c.CarId == carId);

            if (rental != null)
            {
                rentals = rentals.Where(c => c.Id != rental);
            }

            if (rentals != null)//car in rental table
                result = rentals.Any(c => c.StartDate.AddDays(c.RentTerm) >= startDate &&
                 c.StartDate <= startDate.AddDays(rentTerm));

            result = (result == false) ? true : false;
            return result;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateRenatlDto updateRenatlDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            var rentalEntity = await _rentalService.GetRentalByIdAsync(updateRenatlDto.Id);

            if (rentalEntity == null)
                return NotFound(new ApiResponse(404, $"this rental dosnt exists "));

            DateTime startDate = (DateTime)(updateRenatlDto.StartDate.HasValue ? updateRenatlDto.StartDate : rentalEntity.StartDate);
            int rentTerm = (int)(updateRenatlDto.RentTerm.HasValue ? updateRenatlDto.RentTerm : rentalEntity.RentTerm);
            double total = (double)(updateRenatlDto.Total.HasValue ? updateRenatlDto.Total : rentalEntity.Total);
            Guid carId = ((Guid)(updateRenatlDto.CarId.HasValue ? updateRenatlDto.CarId : rentalEntity.CarId));


            if (updateRenatlDto.Total > 0)
            {
                return BadRequest(new ApiResponse(404, $"{updateRenatlDto.Total} must be greater than zero"));
            }

            var result = false;
            if (updateRenatlDto.CarId.HasValue)
            {
                bool resultCarFound = await CheckCarAvaiableAsync((Guid)updateRenatlDto.CarId);
                if (!resultCarFound)
                    return NotFound(new ApiResponse(404, $" car is not found {updateRenatlDto.CarId} "));

                result = CheckCarAvaiableByDateAsync((Guid)updateRenatlDto.CarId, startDate, rentTerm, updateRenatlDto.Id);

                if (!result)
                    return BadRequest(new ApiResponse(400, $"car is not availble in this date {startDate}"));

                rentalEntity.CarId = carId;

            }//car

            if (updateRenatlDto.CustomerId.HasValue)
            {
                var resultCustomerFound = await CheckCustomerAvaiableAsync((Guid)updateRenatlDto.CustomerId);
                if (!resultCustomerFound)
                    return NotFound(new ApiResponse(404, $"customer not found {updateRenatlDto.CustomerId} "));
                rentalEntity.CustomerId = (Guid)updateRenatlDto.CustomerId;
            }

            if (updateRenatlDto.DriverId != null)
            {
                Driver availableDriver = await getAvailbaleDriverAsync((Guid)updateRenatlDto.DriverId,startDate,rentTerm);
                if (availableDriver == null)
                {
                    return NotFound(new ApiResponse(404, $"this driver not available {availableDriver.Name}  "));
                }
                else // driver availble  
                {
                    if (rentalEntity.DriverId != null)
                    {
                        var oldDriver = await _driverService.GetDriverByIdAsync((Guid)rentalEntity.DriverId);
                        oldDriver.IsAvailable = true;
                        await _driverService.UpdateDriverAsync(oldDriver);
                    }

                    availableDriver.IsAvailable = false;
                    await _driverService.UpdateDriverAsync(availableDriver);

                    rentalEntity.DriverId = availableDriver.Id;
                }
            }//driver
            rentalEntity.StartDate = startDate;
            rentalEntity.RentTerm = rentTerm;
            rentalEntity.Total = total;

            if (updateRenatlDto.RentTerm.HasValue)//check car if available after rentterm is updated
            {
                rentTerm = (int)updateRenatlDto.RentTerm;
                bool result1 = CheckCarAvaiableByDateAsync(carId, startDate, rentTerm, updateRenatlDto.Id);
                if (!result1)
                {
                    return BadRequest(new ApiResponse(400, $"car is not availble in this date {startDate}"));
                }
                if (!updateRenatlDto.Total.HasValue)
                {

                    var carRental = await _carService.GetCarByIdAsync(carId);
                    total = (double)(updateRenatlDto.RentTerm * carRental.DailyFare);
                    rentalEntity.Total = total;
                }
            }
            var isUpdated = await _rentalService.UpdateRentalAsync(rentalEntity);
            if (isUpdated)
                return Ok(new ApiResponse(200, $"updated successfuly"));

            return BadRequest(new ApiResponse(403, $" error updated "));

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

                return BadRequest(new ApiResponse(400, $"excption {e}"));
            }
            if (!result)
           return NotFound(new ApiResponse(404, $"this rental dosnt exists "));

            return Ok(new ApiResponse(200,"deleted succcessfuly"));


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
            if (replacmentDriver != null)
            {
                if (replacmentDriver.IsAvailable == true)//replacment Driver is available
                {
                    driver = replacmentDriver;
                }

                else
                {
                    driver = replacmentDriver.ReplacementDriverId != null ? await FindReplacmentDriver((Guid)replacmentDriver.ReplacementDriverId) : null;

                }
            }

            if (driver != null)
            {
                driver.IsAvailable = false;
                await _driverService.UpdateDriverAsync(driver);
            }
            return driver;
        }

        private async Task<bool> CheckCustomerAvaiableAsync(Guid customerId)
        {
            bool result = false;
            var customer = await _customerService.GetCustomerByIdAsync(customerId);

            if (customer != null)
                result = true;

            return result;
        }

        private async Task<bool> CheckCarAvaiableAsync(Guid carId)
        {
            bool result = false;
            var car = await _carService.GetCarByIdAsync(carId);
            if (car != null)
            {
                result = true;
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
        private async Task<Driver> getAvailbaleDriverAsync(Guid driverId,DateTime startDate, int rentTerm)
        {
            var driver = await _driverService.GetDriverByIdAsync(driverId);
            Driver availableDriver = null;
            bool result = false;
            var rentals = _rentalService.GetrentalsQueryable();

            if (driver != null)
            {
                if (rentals != null)//
                { rentals = rentals.Where(c => c.DriverId == driverId);
                    result = rentals.Any(c => c.StartDate.AddDays(c.RentTerm) >= startDate &&
                     c.StartDate <= startDate.AddDays(rentTerm));
                }
                if (driver.IsAvailable && !result)
                {
                    return driver;
                }
                if (driver.ReplacementDriverId != null)
                {
                    availableDriver = await getAvailbaleDriverAsync((Guid)driver.ReplacementDriverId,startDate,rentTerm);
                }
                return availableDriver;
            }
            return driver;
        }
    }
}
