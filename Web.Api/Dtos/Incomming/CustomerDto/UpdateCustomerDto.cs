using System.ComponentModel.DataAnnotations;
using Web.Core.Entities;

namespace Web.Api.Dtos.Incomming.CustomerDto
{
    public class UpdateCustomerDto
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
