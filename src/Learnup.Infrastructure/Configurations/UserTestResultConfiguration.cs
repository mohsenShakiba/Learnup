using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserTestResultConfiguration : IEntityTypeConfiguration<UserTestResult>
{
    public void Configure(EntityTypeBuilder<UserTestResult> builder)
    {
        builder.ToTable("UserTestResult");
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Test)
            .WithMany()
            .HasForeignKey(r => r.TestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.SelectedOption)
            .WithMany()
            .HasForeignKey(r => r.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
