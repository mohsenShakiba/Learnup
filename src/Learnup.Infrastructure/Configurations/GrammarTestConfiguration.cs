using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class GrammarTestConfiguration : IEntityTypeConfiguration<GrammarTest>
{
    public void Configure(EntityTypeBuilder<GrammarTest> builder)
    {
        builder.ToTable("GrammarTest");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Question)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired();

        builder.HasMany(t => t.Options)
            .WithOne()
            .HasForeignKey(o => o.GrammarTestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(t => t.Grammar)
            .WithMany(v => v.Tests)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
