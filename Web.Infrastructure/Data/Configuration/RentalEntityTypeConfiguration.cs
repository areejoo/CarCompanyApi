
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Web.Core.Entities;


namespace Web.Infrastructure.Configuration
{


public class RentalEntityTypeConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        // builder
        //     .Property(r => r.Customer).IsRequired();
            
        // builder
        //     .Property(r => r.Car).IsRequired();

        // builder
        //     .Property(r => r.CreatedAt)
        //     .HasDefaultValueSql("getdate()");

        // builder
        //     .Property(r => r.Status)
        //     .HasDefaultValueSql(StatusRental.Rented.ToString());


        // builder
        //     .Property(r => r.RentTerm)
        //     .IsRequired();
       

    }
}
}