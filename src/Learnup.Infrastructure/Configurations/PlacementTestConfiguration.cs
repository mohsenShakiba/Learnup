using Learnup.Domain.AggregateRoots.Placement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class PlacementTestConfiguration : IEntityTypeConfiguration<PlacementTest>
{
    public void Configure(EntityTypeBuilder<PlacementTest> builder)
    {
        builder.ToTable("PlacementTest");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .IsRequired();

        builder.Property(t => t.Instructions)
            .IsRequired();

        builder.HasMany(t => t.Questions)
            .WithOne()
            .HasForeignKey(q => q.PlacementTestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
