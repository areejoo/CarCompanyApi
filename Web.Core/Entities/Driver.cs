namespace Web.Core.Entities
{
    public class Driver : BaseEntity
    {

        public string Name { get; set; }

        public string Phone { get; set; }

        public bool IsAvailable { get; set; }

        public Guid? ReplacementDriverId { get; set; }
        public Driver ReplacementDriver { get; set; }

        public virtual ICollection<Car> Cars { get; set; }

        public virtual ICollection<Rental> Rentals { get; set; }

        public virtual ICollection<Driver> ReplacementDrivers { get; set; }

    }
}