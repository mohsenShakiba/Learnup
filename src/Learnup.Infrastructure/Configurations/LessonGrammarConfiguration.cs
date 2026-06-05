using Learnup.Domain.AggregateRoots.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class LessonGrammarConfiguration : IEntityTypeConfiguration<LessonGrammar>
{
    public void Configure(EntityTypeBuilder<LessonGrammar> builder)
    {
        builder.ToTable("LessonGrammar");

        builder.HasKey(lg => new { lg.LessonId, lg.GrammarId });

        builder.HasOne(lg => lg.Lesson)
            .WithMany(l => l.Grammars)
            .HasForeignKey(lg => lg.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lg => lg.Grammar)
            .WithMany()
            .HasForeignKey(lg => lg.GrammarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
