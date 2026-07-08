using Learnup.Domain.AggregateRoots.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class StoryItemExpressionConfiguration : IEntityTypeConfiguration<StoryItemExpression>
{
    public void Configure(EntityTypeBuilder<StoryItemExpression> builder)
    {
        builder.ToTable("StoryItemExpression");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Phrase)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(e => e.Meaning)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(e => e.Translation)
            .HasMaxLength(2000);

        builder.HasOne(e => e.StoryItem)
            .WithMany(si => si.Expressions)
            .HasForeignKey(e => e.StoryItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
