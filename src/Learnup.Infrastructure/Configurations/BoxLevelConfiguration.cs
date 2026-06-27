using Learnup.Domain.AggregateRoots.LeitnerBoxes;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class BoxLevelConfiguration : IEntityTypeConfiguration<BoxLevel>
{
    public void Configure(EntityTypeBuilder<BoxLevel> builder)
    {
        builder.ToTable("BoxLevel");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Level).IsRequired();
        builder.Property(l => l.WillReviewedIn).IsRequired();

        builder.HasOne(l => l.LeitnerBox)
            .WithMany(lb => lb.BoxLevels)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Items)
            .WithOne(i => i.BoxLevel)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
