using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class VocabConfiguration : IEntityTypeConfiguration<Vocab>
{
    public void Configure(EntityTypeBuilder<Vocab> builder)
    {
        builder.ToTable("Vocab");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Word)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(v => v.Translation)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(v => v.VoiceId)
            .HasMaxLength(255);

        builder.Property(v => v.Description)
            .HasMaxLength(1000);

        builder.HasOne(v => v.Language)
            .WithMany()
            .HasForeignKey(v => v.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

  
    }
}
