using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class VocabTestConfiguration : IEntityTypeConfiguration<VocabTest>
{
    public void Configure(EntityTypeBuilder<VocabTest> builder)
    {
        builder.ToTable("VocabTest");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Question)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired();

        builder.HasMany(t => t.Options)
            .WithOne()
            .HasForeignKey(o => o.VocabTestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(t => t.Vocab)
            .WithMany(v => v.Tests)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
