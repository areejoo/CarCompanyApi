using System.ComponentModel.DataAnnotations;
using Web.Core.Entities;
using Web.Core.Enums;

namespace Web.Api.Dtos.Outcomming.RentalDto
{
    public class RentalDto
    {
        
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public  string  CustomerName { get; set; }
        public Guid CarId { get; set; }
        public int CarNumber { get; set; }
        public Guid? DriverId { get; set; }
        public string DriverName { get; set; }
        public RentalStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public int RentTerm { get; set; }
        public double Total { get; set; }

    }
}
