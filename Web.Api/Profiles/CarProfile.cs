using AutoMapper;
using web.api.Dtos.Incomming;
using Web.Api.Dtos.Incomming;
using Web.Api.Dtos.Outcomming;
using Web.Core.Entities;

namespace Web.APi.Profilles
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
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
