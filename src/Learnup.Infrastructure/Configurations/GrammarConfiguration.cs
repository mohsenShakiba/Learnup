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

        builder.Property(g => g.Description)
            .IsRequired();

        builder.Property(g => g.Level)
            .IsRequired();

        builder.Property(g => g.Order)
            .IsRequired();

        builder.Property(g => g.EstimatedTime)
            .IsRequired();

        builder.HasMany(g => g.Lessons)
            .WithOne(g => g.Grammar)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(g => g.Lessons)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(g => g.ParentGrammar)
            .WithMany(g => g.PrerequisiteGrammars)
            .HasForeignKey(g => g.ParentGrammarId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Navigation(g => g.PrerequisiteGrammars)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
