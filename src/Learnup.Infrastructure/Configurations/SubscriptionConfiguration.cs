using Learnup.Domain.AggregateRoots.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(1000).IsRequired();
        builder.Property(s => s.Type).IsRequired();
        builder.Property(s => s.Duration).IsRequired();
        builder.Property(s => s.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(s => s.DiscountPercent).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(s => s.IsActive).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.HasMany(s => s.Features)
            .WithOne(f => f.Subscription)
            .HasForeignKey(f => f.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.UserSubscriptions)
            .WithOne(us => us.Subscription)
            .HasForeignKey(us => us.SubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
