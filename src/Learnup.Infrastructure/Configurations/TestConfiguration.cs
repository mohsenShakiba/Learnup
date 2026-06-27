using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.ToTable("Test");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Question)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(t => t.Type)
            .IsRequired();

        builder.Property(t => t.QuestionType)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired();

        builder.HasOne(t => t.Lesson)
            .WithMany()
            .HasForeignKey(t => t.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Options)
            .WithOne()
            .HasForeignKey(o => o.TestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
