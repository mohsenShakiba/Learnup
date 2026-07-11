using Learnup.Domain.AggregateRoots.AudioBooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class AudioBookListItemConfiguration : IEntityTypeConfiguration<AudioBookListItem>
{
    public void Configure(EntityTypeBuilder<AudioBookListItem> builder)
    {
        builder.ToTable("AudioBookListItem");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Sentence)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(item => item.Translation)
            .HasMaxLength(2000);

        builder.HasOne(item => item.AudioBook)
            .WithMany(audioBook => audioBook.Items)
            .HasForeignKey(item => item.AudioBookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(item => item.Expressions)
            .WithOne(expression => expression.AudioBookListItem)
            .HasForeignKey(expression => expression.AudioBookListItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
