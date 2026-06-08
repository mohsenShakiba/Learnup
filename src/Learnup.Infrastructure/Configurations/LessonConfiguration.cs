using Learnup.Domain.AggregateRoots.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lesson");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(l => l.Order)
            .IsRequired();

        builder.Property(l => l.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Stories)
            .WithOne(ls => ls.Lesson)
            .HasForeignKey(ls => ls.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Grammars)
            .WithOne(lg => lg.Lesson)
            .HasForeignKey(lg => lg.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Vocabs)
            .WithOne(lv => lv.Lesson)
            .HasForeignKey(lv => lv.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(l => l.Users)
            .WithOne(lv => lv.Lesson)
            .HasForeignKey(lv => lv.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
