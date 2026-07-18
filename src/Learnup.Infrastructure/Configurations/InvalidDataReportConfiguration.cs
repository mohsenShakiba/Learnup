using Learnup.Domain.AggregateRoots.InvalidData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class InvalidDataReportConfiguration : IEntityTypeConfiguration<InvalidDataReport>
{
    public void Configure(EntityTypeBuilder<InvalidDataReport> builder)
    {
        builder.ToTable("InvalidDataReport");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Kind)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.Section)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.CourseId)
            .IsRequired(false);

        builder.Property(r => r.LessonId)
            .IsRequired(false);

        builder.Property(r => r.TargetId)
            .IsRequired(false);

        builder.Property(r => r.TargetType)
            .HasConversion<int>()
            .IsRequired(false);

        builder.Property(r => r.Text)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.Section);
        builder.HasIndex(r => r.CreatedAt);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
