using AutoMapper;
using web.api.Dtos.Incomming;
using Web.Api.Dtos.Incomming;
using Web.Api.Dtos.Incomming.CustomerDto;
using Web.Api.Dtos.Incomming.DriverDto;
using Web.Api.Dtos.Outcomming;
using Web.Api.Dtos.Outcomming.CustomerDto;
using Web.Api.Dtos.Outcomming.DriverDto;
using Web.Core.Entities;

namespace Web.APi.Profilles
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {//
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<Customer, CreateCustomerDto>();
            CreateMap<UpdateCustomerDto, CustomerDto>();
            CreateMap<UpdateCustomerDto, Customer>();
            //

            CreateMap<Driver, DriverDto>();
            CreateMap<DriverDto, Driver>();
            CreateMap<CreateDriverDto, Driver>();
            CreateMap<Driver, CreateDriverDto>();
            CreateMap<UpdateCarDto, DriverDto>();
            CreateMap<UpdateDriverDto, Driver>();

            CreateMap<Car, CarDto>();
            CreateMap<CarDto, Car>();
            CreateMap<CreateCarDto, Car>();
            CreateMap<Car, CreateCarDto>();
            CreateMap<UpdateCarDto,CarDto>();
            CreateMap<UpdateCarDto, Car>();
               // .ForMember(destination => destination.Type, options => options.Condition(source => !string.IsNullOrEmpty(source.Type)));



          
        }
    }
}
