using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class VocabTestOptionConfiguration : IEntityTypeConfiguration<VocabTestOption>
{
    public void Configure(EntityTypeBuilder<VocabTestOption> builder)
    {
        builder.ToTable("VocabTestOption");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Text)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(o => o.IsCorrect)
            .IsRequired();
    }
}
