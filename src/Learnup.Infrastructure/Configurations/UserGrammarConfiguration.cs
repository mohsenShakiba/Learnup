using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserGrammarConfiguration : IEntityTypeConfiguration<UserGrammar>
{
    public void Configure(EntityTypeBuilder<UserGrammar> builder)
    {
        builder.ToTable("UserGrammar");

        builder.HasKey(ug => new { ug.UserId, ug.GrammarId });

        builder.Property(ug => ug.StartedAt)
            .IsRequired();

        builder.Property(ug => ug.CompletedAt);

        builder.HasOne(ug => ug.User)
            .WithMany(u => u.Grammars)
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ug => ug.Grammar)
            .WithMany()
            .HasForeignKey(ug => ug.GrammarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
