using Learnup.Domain.AggregateRoots.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class LessonConversationConfiguration : IEntityTypeConfiguration<LessonConversation>
{
    public void Configure(EntityTypeBuilder<LessonConversation> builder)
    {
        builder.ToTable("LessonConversation");

        builder.HasKey(ls => new { ls.LessonId, ls.ConversationId });

        builder.HasOne(ls => ls.Lesson)
            .WithMany(l => l.Conversations)
            .HasForeignKey(ls => ls.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ls => ls.Conversation)
            .WithMany(s => s.Lessons)
            .HasForeignKey(ls => ls.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
