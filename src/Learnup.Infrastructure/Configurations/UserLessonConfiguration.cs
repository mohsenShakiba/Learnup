using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserLessonConfiguration : IEntityTypeConfiguration<UserLesson>
{
    public void Configure(EntityTypeBuilder<UserLesson> builder)
    {
        builder.ToTable("UserLesson");

        builder.HasKey(ul => new { ul.UserId, ul.LessonId });

        builder.Property(ul => ul.StartedAt)
            .IsRequired();

        builder.Property(ul => ul.LastVisitedAt)
            .IsRequired();

        builder.Property(ul => ul.CompletedAt);

        builder.Property(ul => ul.Status)
            .IsRequired();

        builder.Ignore(ul => ul.HasStory);
        builder.Ignore(ul => ul.HasGrammar);
        builder.Ignore(ul => ul.HasVocab);
        builder.Ignore(ul => ul.HasTest);

        builder.HasOne(ul => ul.User)
            .WithMany(u => u.Lessons)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ul => ul.Lesson)
            .WithMany(l => l.Users)
            .HasForeignKey(ul => ul.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
