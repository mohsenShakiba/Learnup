using Learnup.Domain.AggregateRoots.Conversations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class ConversationItemVoiceTimingConfiguration : IEntityTypeConfiguration<ConversationItemVoiceTiming>
{
    public void Configure(EntityTypeBuilder<ConversationItemVoiceTiming> builder)
    {
        builder.ToTable("ConversationItemVoiceTiming");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Text)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(t => t.StartSeconds)
            .IsRequired();

        builder.Property(t => t.EndSeconds)
            .IsRequired();

        builder.HasOne(t => t.ConversationItem)
            .WithMany(si => si.VoiceTimings)
            .HasForeignKey(t => t.ConversationItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
