using Learnup.Domain.AggregateRoots.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class StoryConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.ToTable("Story");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasMany(s => s.Items)
            .WithOne(si => si.Story)
            .HasForeignKey(si => si.StoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
