using Learnup.Domain.AggregateRoots.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class LessonStoryConfiguration : IEntityTypeConfiguration<LessonStory>
{
    public void Configure(EntityTypeBuilder<LessonStory> builder)
    {
        builder.ToTable("LessonStory");

        builder.HasKey(ls => new { ls.LessonId, ls.StoryId });

        builder.HasOne(ls => ls.Lesson)
            .WithMany(l => l.Stories)
            .HasForeignKey(ls => ls.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ls => ls.Story)
            .WithMany(s => s.Lessons)
            .HasForeignKey(ls => ls.StoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
