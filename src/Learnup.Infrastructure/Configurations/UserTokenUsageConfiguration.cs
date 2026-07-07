using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserTokenUsageConfiguration : IEntityTypeConfiguration<UserTokenUsage>
{
    public void Configure(EntityTypeBuilder<UserTokenUsage> builder)
    {
        builder.ToTable("UserTokenUsage");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.PromptTokens)
            .IsRequired();

        builder.Property(u => u.CompletionTokens)
            .IsRequired();

        builder.Property(u => u.TotalTokens)
            .IsRequired();

        builder.Property(u => u.RequestCount)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .IsRequired();

        builder.HasIndex(u => u.UserId)
            .IsUnique();

        builder.HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
