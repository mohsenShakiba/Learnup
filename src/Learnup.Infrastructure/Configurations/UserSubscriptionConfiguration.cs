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

        builder.HasOne(us => us.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
