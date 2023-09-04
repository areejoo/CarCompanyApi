using System.ComponentModel.DataAnnotations;
using Web.Core.Entities;
using Web.Core.Enums;

namespace Web.Api.Dtos.Incomming.RentalDto
{
    public class CreateRentalDto
    {
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public Guid CarId { get; set; }
      
        public Guid? DriverId { get; set; }

        public RentalStatus Status { get; set; }

        public DateTime StartDate { get; set; }
        [Required]
        public int RentTerm { get; set; }

        public double Total { get; set; }

    }
}
