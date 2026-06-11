using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class VocabTransactionConfiguration : IEntityTypeConfiguration<VocabTransaction>
{
    public void Configure(EntityTypeBuilder<VocabTransaction> builder)
    {
        builder.ToTable("VocabTransaction");

        builder.HasKey(vt => vt.Id);

        builder.Property(vt => vt.Translation)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(vt => vt.Description)
            .HasMaxLength(1000);

        builder.Property(vt => vt.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(vt => vt.Example)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(vt => vt.ExampleTranslation)
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasOne(vt => vt.Vocab)
            .WithMany()
            .HasForeignKey(vt => vt.VocabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
