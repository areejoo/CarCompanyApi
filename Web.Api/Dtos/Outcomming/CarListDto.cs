using System.ComponentModel.DataAnnotations;
using Web.Core.Entities;

namespace Web.Api.Dtos.Outcomming
{
    public class CarListDto
    {

        public IReadOnlyList<CarDto> CarsPaginationList { get; set; }

        public int Count { get; set; }
    }
}