using Learnup.Domain.AggregateRoots.Grammars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class GrammarLessonConfiguration : IEntityTypeConfiguration<GrammarLesson>
{
    public void Configure(EntityTypeBuilder<GrammarLesson> builder)
    {
        builder.ToTable("GrammarLesson");

        builder.HasKey(gl => gl.Id);

        builder.Property<int>("GrammarId");

        builder.Property(gl => gl.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(gl => gl.Content)
            .IsRequired();

        builder.Property(gl => gl.Order)
            .IsRequired();

        builder.Property(gl => gl.Language)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(gl => gl.HtmlTag)
            .IsRequired();
    }
}
