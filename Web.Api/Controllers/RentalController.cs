using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Dtos.Incomming.CustomerDto;
using Web.Api.Dtos.Outcomming.CustomerDto;
using Web.Core.Interfaces;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : Controller
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IMapper _mapper;

        public RentalController(IRentalRepository rentalRepository, IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _mapper = mapper;
        }

        public async Task<RentalListDto> GetListAsync([FromQuery] RentalDtoRequest request)

        {
            return View();
        }
    }
}
