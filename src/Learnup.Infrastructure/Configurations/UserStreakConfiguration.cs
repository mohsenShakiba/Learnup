using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserStreakConfiguration : IEntityTypeConfiguration<UserStreak>
{
    public void Configure(EntityTypeBuilder<UserStreak> builder)
    {
        builder.ToTable("UserStreak");

        builder.HasKey(us => us.Id);

        builder.Property(us => us.StreakDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(us => us.VisitedAt)
            .IsRequired();

        builder.HasIndex(us => new { us.UserId, us.StreakDate })
            .IsUnique();

        builder.HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
