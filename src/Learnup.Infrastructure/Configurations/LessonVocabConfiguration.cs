using Learnup.Domain.AggregateRoots.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class LessonVocabConfiguration : IEntityTypeConfiguration<LessonVocab>
{
    public void Configure(EntityTypeBuilder<LessonVocab> builder)
    {
        builder.ToTable("LessonVocab");

        builder.HasKey(lv => new { lv.LessonId, lv.VocabId });

        builder.HasOne(lv => lv.Lesson)
            .WithMany(l => l.Vocabs)
            .HasForeignKey(lv => lv.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lv => lv.Vocab)
            .WithMany()
            .HasForeignKey(lv => lv.VocabId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
