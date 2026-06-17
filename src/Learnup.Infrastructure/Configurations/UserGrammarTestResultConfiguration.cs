using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserGrammarTestResultConfiguration : IEntityTypeConfiguration<UserGrammarTestResult>
{
    public void Configure(EntityTypeBuilder<UserGrammarTestResult> builder)
    {
        builder.ToTable("UserGrammarTestResult");
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.GrammarTest)
            .WithMany()
            .HasForeignKey(r => r.GrammarTestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.SelectedOption)
            .WithMany()
            .HasForeignKey(r => r.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
