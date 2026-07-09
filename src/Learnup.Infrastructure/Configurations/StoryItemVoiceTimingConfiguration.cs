using Learnup.Domain.AggregateRoots.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class StoryItemVoiceTimingConfiguration : IEntityTypeConfiguration<StoryItemVoiceTiming>
{
    public void Configure(EntityTypeBuilder<StoryItemVoiceTiming> builder)
    {
        builder.ToTable("StoryItemVoiceTiming");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Text)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(t => t.StartSeconds)
            .IsRequired();

        builder.Property(t => t.EndSeconds)
            .IsRequired();

        builder.HasOne(t => t.StoryItem)
            .WithMany(si => si.VoiceTimings)
            .HasForeignKey(t => t.StoryItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
