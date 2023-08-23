using System.ComponentModel.DataAnnotations;
using Web.Core.Enums;

namespace Web.Core.Entities
{
    public class Rental : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid CarId { get; set; }
        public Car Car { get; set; }

        public Guid? DriverId { get; set; }
        public Driver Driver { get; set; }

        public RentalStatus Status { get; set; }

        public DateTime StartDate { get; set; }
        public int RentTerm { get; set; }

        public double Total { get; set; }

    }
}
