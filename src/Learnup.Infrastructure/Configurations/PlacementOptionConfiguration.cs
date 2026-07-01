using Learnup.Domain.AggregateRoots.Placement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class PlacementOptionConfiguration : IEntityTypeConfiguration<PlacementOption>
{
    public void Configure(EntityTypeBuilder<PlacementOption> builder)
    {
        builder.ToTable("PlacementOption");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Text)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(o => o.IsCorrect)
            .IsRequired();
    }
}
