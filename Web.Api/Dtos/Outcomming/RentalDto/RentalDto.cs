﻿using System.ComponentModel.DataAnnotations;
using Web.Core.Entities;
using Web.Core.Enums;

namespace Web.Api.Dtos.Outcomming.RentalDto
{
    public class RentalDto
    {
        
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CarId { get; set; }
        public Guid? DriverId { get; set; }
        public RentalStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public int RentTerm { get; set; }
        public double Total { get; set; }

    }
}
