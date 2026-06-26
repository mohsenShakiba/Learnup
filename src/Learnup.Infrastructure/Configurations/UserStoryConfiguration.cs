using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserStoryConfiguration : IEntityTypeConfiguration<UserStory>
{
    public void Configure(EntityTypeBuilder<UserStory> builder)
    {
        builder.ToTable("UserStory");

        builder.HasKey(us => new { us.UserId, us.StoryId });

        builder.Property(us => us.StartedAt)
            .IsRequired();

        builder.Property(us => us.CompletedAt);

        builder.HasOne(us => us.User)
            .WithMany(u => u.Stories)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(us => us.Story)
            .WithMany()
            .HasForeignKey(us => us.StoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
