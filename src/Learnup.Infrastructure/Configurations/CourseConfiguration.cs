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

        builder.Property(c => c.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Order)
            .IsRequired();

        builder.HasOne(c => c.Language)
            .WithMany()
            .HasForeignKey(c => c.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
