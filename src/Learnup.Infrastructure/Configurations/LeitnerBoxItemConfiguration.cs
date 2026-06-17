using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class LeitnerBoxItemConfiguration : IEntityTypeConfiguration<LeitnerBoxItem>
{
    public void Configure(EntityTypeBuilder<LeitnerBoxItem> builder)
    {
        builder.ToTable("LeitnerBoxItem");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.LeitnerBoxId).IsRequired();
        builder.Property(i => i.BoxLevelId).IsRequired();
        builder.Property(i => i.VocabId).IsRequired();
        builder.Property(i => i.AddedAt).IsRequired();
        builder.Property(i => i.NextReviewAt).IsRequired(false);
        builder.Property(i => i.ReviewedAt).IsRequired(false);

        builder.HasOne(i => i.LeitnerBox)
            .WithMany(b => b.Items)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.BoxLevel)
            .WithMany(l => l.Items)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Vocab)
            .WithMany()
            .HasForeignKey(i => i.VocabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
