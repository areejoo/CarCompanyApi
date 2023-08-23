using Web.Core.Entities;

namespace Web.Api.Dtos.Outcomming.DriverDto
{
    public class DriverDto
    {
       
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Phone { get; set; }

        public bool IsAvailable { get; set; }

        public Guid? ReplacementDriverId { get; set; }

        public virtual ICollection<Car> Cars { get; set; }

        public virtual ICollection<Rental> Rentals { get; set; }

        public virtual ICollection<Driver> ReplacementDrivers { get; set; }
    }
}
