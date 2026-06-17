using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserVocabTestResultConfiguration : IEntityTypeConfiguration<UserVocabTestResult>
{
    public void Configure(EntityTypeBuilder<UserVocabTestResult> builder)
    {
        builder.ToTable("UserVocabTestResult");
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.VocabTest)
            .WithMany()
            .HasForeignKey(r => r.VocabTestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.SelectedOption)
            .WithMany()
            .HasForeignKey(r => r.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
