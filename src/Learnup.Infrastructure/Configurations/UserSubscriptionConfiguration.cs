using Learnup.Domain.AggregateRoots.Subscriptions;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions");
        builder.HasKey(us => us.Id);

        builder.Property(us => us.StartedAt).IsRequired();
        builder.Property(us => us.ExpiresAt).IsRequired();
        builder.Property(us => us.Status).IsRequired();
        builder.Property(us => us.CreatedAt).IsRequired();

        builder.HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(us => us.Subscription)
            .WithMany(s => s.UserSubscriptions)
            .HasForeignKey(us => us.SubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
