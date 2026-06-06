using Learnup.Domain.AggregateRoots.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class StoryItemTimestampConfiguration : IEntityTypeConfiguration<StoryItemTimestamp>
{
    public void Configure(EntityTypeBuilder<StoryItemTimestamp> builder)
    {
        builder.ToTable("StoryItemTimestamp");

        builder.HasKey(timestamp => timestamp.Id);

        builder.Property(timestamp => timestamp.Word)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(timestamp => timestamp.Start)
            .IsRequired();

        builder.Property(timestamp => timestamp.End)
            .IsRequired();
    }
}
