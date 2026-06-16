using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class VocabTranslationConfiguration : IEntityTypeConfiguration<VocabTranslation>
{
    public void Configure(EntityTypeBuilder<VocabTranslation> builder)
    {
        builder.ToTable("VocabTranslation");

        builder.HasKey(vt => vt.Id);

        builder.Property(vt => vt.Translation)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(vt => vt.Description)
            .HasMaxLength(1000);

        builder.HasOne(vt => vt.Vocab)
            .WithMany(v => v.Translations)
            .HasForeignKey(vt => vt.VocabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}