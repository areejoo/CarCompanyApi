using Microsoft.AspNetCore.Mvc;
using Web.Api.Dtos;
using Web.Api.Dtos.Incomming;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class testController : Controller
    {
        
        [HttpPost]
        public IActionResult Create([FromBody] MyDto createCarDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
            
        }
    }
}
