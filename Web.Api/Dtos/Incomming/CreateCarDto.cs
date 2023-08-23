using System.ComponentModel.DataAnnotations;
namespace Web.Api.Dtos.Incomming
{
    public class CreateCarDto

    {
        [Required(ErrorMessage ="number is required")]
        public int Number { get; set; }

        public double EngineCapacity { get; set; }
        [StringLength(15)]
        public string Color { get; set; }

        [StringLength(10)]
        public string Type { get; set; }


        public double DailyFare { get; set; }

        public Guid? DriverId { get; set; }
        
       


    }
}
