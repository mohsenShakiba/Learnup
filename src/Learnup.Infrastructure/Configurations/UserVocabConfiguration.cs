using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserVocabConfiguration : IEntityTypeConfiguration<UserVocab>
{
    public void Configure(EntityTypeBuilder<UserVocab> builder)
    {
        builder.ToTable("UserVocab");

        builder.HasKey(uv => new { uv.UserId, uv.VocabId });

        builder.Property(uv => uv.StartedAt)
            .IsRequired();

        builder.Property(uv => uv.CompletedAt);

        builder.HasOne(uv => uv.User)
            .WithMany(u => u.Vocabs)
            .HasForeignKey(uv => uv.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uv => uv.Vocab)
            .WithMany()
            .HasForeignKey(uv => uv.VocabId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
