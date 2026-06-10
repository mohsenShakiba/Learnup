using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserCourseConfiguration : IEntityTypeConfiguration<UserCourse>
{
    public void Configure(EntityTypeBuilder<UserCourse> builder)
    {
        builder.ToTable("UserCourse");

        builder.HasKey(uc => new { uc.UserId, uc.CourseId });

        builder.Property(uc => uc.FirstVisitedAt)
            .IsRequired();

        builder.HasOne(uc => uc.User)
            .WithMany()
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uc => uc.Course)
            .WithMany(c => c.Users)
            .HasForeignKey(uc => uc.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
