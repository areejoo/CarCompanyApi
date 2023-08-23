using System.ComponentModel.DataAnnotations;

namespace Web.Api.Dtos.Outcomming
{
    public class CarDto
    {
        [Required]
        public Guid Id { get; set; }

       
        public int Number { get; set; }

        public double EngineCapacity { get; set; }

        public string Color { get; set; }

        public string Type { get; set; }

        public double DailyFare { get; set; }

        public bool WithDriver { get; set; }

        public Guid? DriverId { get; set; }

    }
}