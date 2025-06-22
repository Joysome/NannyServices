using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nanny.Admin.Domain.Entities;

namespace Nanny.Admin.Infrastructure.Configurations;

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.HasKey(ol => ol.Id);

        builder.Property(ol => ol.Id)
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(ol => ol.OrderId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(ol => ol.ProductId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(ol => ol.Count)
            .IsRequired();

        builder.Property(ol => ol.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(ol => ol.Order)
            .WithMany(o => o.OrderLines)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ol => ol.Product)
            .WithMany()
            .HasForeignKey(ol => ol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for better query performance
        builder.HasIndex(ol => ol.OrderId);
        builder.HasIndex(ol => ol.ProductId);
        builder.HasIndex(ol => new { ol.OrderId, ol.ProductId });
    }
}
