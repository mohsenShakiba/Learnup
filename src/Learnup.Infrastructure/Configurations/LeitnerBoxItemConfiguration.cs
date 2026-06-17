using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class LeitnerBoxItemConfiguration : IEntityTypeConfiguration<LeitnerBoxItem>
{
    public void Configure(EntityTypeBuilder<LeitnerBoxItem> builder)
    {
        builder.ToTable("LeitnerBoxItem");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.BoxLevel).IsRequired();
        builder.Property(i => i.AddedAt).IsRequired();

        builder.HasOne(i => i.Vocab)
            .WithMany()
            .HasForeignKey(i => i.VocabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
