using Learnup.Domain.AggregateRoots.AudioBooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class AudioBookListItemExpressionConfiguration : IEntityTypeConfiguration<AudioBookListItemExpression>
{
    public void Configure(EntityTypeBuilder<AudioBookListItemExpression> builder)
    {
        builder.ToTable("AudioBookListItemExpression");

        builder.HasKey(expression => expression.Id);

        builder.Property(expression => expression.Phrase)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(expression => expression.Meaning)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(expression => expression.Translation)
            .HasMaxLength(2000);

        builder.HasOne(expression => expression.AudioBookListItem)
            .WithMany(item => item.Expressions)
            .HasForeignKey(expression => expression.AudioBookListItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
