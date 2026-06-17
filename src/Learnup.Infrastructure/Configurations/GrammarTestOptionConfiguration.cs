using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class GrammarTestOptionConfiguration : IEntityTypeConfiguration<GrammarTestOption>
{
    public void Configure(EntityTypeBuilder<GrammarTestOption> builder)
    {
        builder.ToTable("GrammarTestOption");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Text)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(o => o.IsCorrect)
            .IsRequired();
    }
}
