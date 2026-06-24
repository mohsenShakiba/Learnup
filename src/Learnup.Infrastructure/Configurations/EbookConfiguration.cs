using Learnup.Domain.AggregateRoots.Ebooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class EbookConfiguration : IEntityTypeConfiguration<Ebook>
{
    public void Configure(EntityTypeBuilder<Ebook> builder)
    {
        builder.ToTable("Ebook");

        builder.HasKey(ebook => ebook.Id);

        builder.Property(ebook => ebook.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ebook => ebook.Author)
            .HasMaxLength(500);

        builder.Property(ebook => ebook.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ebook => ebook.CoverId)
            .HasMaxLength(500);

        builder.Property(ebook => ebook.UploadedAt)
            .IsRequired();
    }
}
