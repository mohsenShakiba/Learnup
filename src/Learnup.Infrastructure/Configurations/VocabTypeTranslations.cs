using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class VocabTypeTranslations: IEntityTypeConfiguration<VocabSense>
{
    public void Configure(EntityTypeBuilder<VocabSense> builder)
    {
        builder.ToTable("VocabSense");

        builder.HasKey(v => v.Id);

        builder.HasOne(v => v.Vocab)
            .WithMany(v => v.Senses)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(v => v.Translation)
            .HasMaxLength(1024)
            .IsRequired();
        
        builder.Property(v => v.Example)
            .HasMaxLength(1024)
            .IsRequired();
        
        builder.Property(v => v.ExampleTranslation)
            .HasMaxLength(1024)
            .IsRequired();
    }
}