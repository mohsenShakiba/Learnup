using Learnup.Domain.AggregateRoots.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Course");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Slug)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Code)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(c => c.Title)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(c => c.Description)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(c => c.Order)
            .IsRequired();

        builder.HasOne(c => c.Language)
            .WithMany()
            .HasForeignKey(c => c.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(c => c.Lessons)
            .WithOne(l => l.Course)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
