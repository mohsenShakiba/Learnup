using Learnup.Domain.AggregateRoots.Grammars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class GrammarConfiguration : IEntityTypeConfiguration<Grammar>
{
    public void Configure(EntityTypeBuilder<Grammar> builder)
    {
        builder.ToTable("Grammar");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
