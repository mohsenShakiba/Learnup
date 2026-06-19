using Learnup.Domain.AggregateRoots.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class SubscriptionFeatureConfiguration : IEntityTypeConfiguration<SubscriptionFeature>
{
    public void Configure(EntityTypeBuilder<SubscriptionFeature> builder)
    {
        builder.ToTable("SubscriptionFeatures");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Description).HasMaxLength(500).IsRequired();
        builder.Property(f => f.IsIncluded).IsRequired();
        builder.Property(f => f.Order).IsRequired();
    }
}
