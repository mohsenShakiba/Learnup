using Learnup.Domain.AggregateRoots.MotivationalSentences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class MotivationalSentenceConfiguration : IEntityTypeConfiguration<MotivationalSentence>
{
    public void Configure(EntityTypeBuilder<MotivationalSentence> builder)
    {
        builder.ToTable("MotivationalSentences");

        builder.HasKey(sentence => sentence.Id);

        builder.Property(sentence => sentence.Sentence)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(sentence => sentence.IsActive)
            .IsRequired();

        builder.HasData(
            new { Id = 1, Sentence = "Small steps every day become meaningful progress.", IsActive = true },
            new { Id = 2, Sentence = "Stay consistent; skill grows when effort repeats.", IsActive = true },
            new { Id = 3, Sentence = "One focused lesson today makes tomorrow easier.", IsActive = true },
            new { Id = 4, Sentence = "Mistakes are proof that practice is happening.", IsActive = true },
            new { Id = 5, Sentence = "Keep going; fluency is built one choice at a time.", IsActive = true });
    }
}
