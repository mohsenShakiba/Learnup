using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class LeitnerBoxConfiguration : IEntityTypeConfiguration<LeitnerBox>
{
    public void Configure(EntityTypeBuilder<LeitnerBox> builder)
    {
        builder.ToTable("LeitnerBox");
        builder.HasKey(b => b.Id);

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Items)
            .WithOne(i => i.LeitnerBox)
            .HasForeignKey(i => i.LeitnerBoxId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
