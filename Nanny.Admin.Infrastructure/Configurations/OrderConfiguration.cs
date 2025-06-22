using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Domain.Enums;

namespace Nanny.Admin.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(o => o.CustomerId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v));

        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(o => o.UpdatedAt)
            .IsRequired(false)
            .HasColumnType("datetime2");

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.OrderLines)
            .WithOne(ol => ol.Order)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for better query performance
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
        builder.HasIndex(o => new { o.CustomerId, o.Status });
        builder.HasIndex(o => new { o.Status, o.CreatedAt });
    }
}
