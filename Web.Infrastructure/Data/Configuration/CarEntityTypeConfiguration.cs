
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Web.Core.Entities;


namespace Web.Infrastructure.Configuration
{


    public class CarEntityTypeConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder
                .HasIndex(c => c.Number)
                .IsUnique();

            builder
               .Property(c => c.Id)
                .HasDefaultValueSql("NewID()");
        }
    }
}