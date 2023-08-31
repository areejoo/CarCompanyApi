namespace Web.Core.Entities
{
    public class Car : BaseEntity
    {

        public int Number { get; set; }

        public double EngineCapacity { get; set; }

        public string Color { get; set; }

        public string Type { get; set; }

        public double DailyFare { get; set; }

        public bool WithDriver { get; set; }

        public Guid? DriverId { get; set; }//titleCase


        public Driver Driver { get; set; }

        public bool IsAvailable { get; set; }
        public virtual ICollection<Rental> Rentals { get; set; }

    }
}