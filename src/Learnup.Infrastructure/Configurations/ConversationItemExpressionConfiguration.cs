using Learnup.Domain.AggregateRoots.Conversations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class ConversationItemExpressionConfiguration : IEntityTypeConfiguration<ConversationItemExpression>
{
    public void Configure(EntityTypeBuilder<ConversationItemExpression> builder)
    {
        builder.ToTable("ConversationItemExpression");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Phrase)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(e => e.Meaning)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(e => e.Translation)
            .HasMaxLength(2000);

        builder.HasOne(e => e.ConversationItem)
            .WithMany(si => si.Expressions)
            .HasForeignKey(e => e.ConversationItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
