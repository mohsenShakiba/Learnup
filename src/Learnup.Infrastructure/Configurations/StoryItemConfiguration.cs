using Learnup.Domain.AggregateRoots.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class StoryItemConfiguration : IEntityTypeConfiguration<StoryItem>
{
    public void Configure(EntityTypeBuilder<StoryItem> builder)
    {
        builder.ToTable("StoryItem");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.Content)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(si => si.Translation)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(si => si.VoiceId)
            .HasMaxLength(255);

        builder.HasOne(si => si.Story)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

    
    }
}
