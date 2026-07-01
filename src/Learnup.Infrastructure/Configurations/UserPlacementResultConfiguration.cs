using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserPlacementResultConfiguration : IEntityTypeConfiguration<UserPlacementResult>
{
    public void Configure(EntityTypeBuilder<UserPlacementResult> builder)
    {
        builder.ToTable("UserPlacementResult");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.PlacedLevel)
            .HasMaxLength(10)
            .IsRequired();

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.UserId)
            .IsUnique();

        builder.HasMany(r => r.Answers)
            .WithOne()
            .HasForeignKey(a => a.UserPlacementResultId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
