using Learnup.Domain.AggregateRoots.Placement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class PlacementQuestionConfiguration : IEntityTypeConfiguration<PlacementQuestion>
{
    public void Configure(EntityTypeBuilder<PlacementQuestion> builder)
    {
        builder.ToTable("PlacementQuestion");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Number)
            .IsRequired();

        builder.Property(q => q.Level)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(q => q.Skill)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(q => q.Prompt)
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasMany(q => q.Options)
            .WithOne()
            .HasForeignKey(o => o.PlacementQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
