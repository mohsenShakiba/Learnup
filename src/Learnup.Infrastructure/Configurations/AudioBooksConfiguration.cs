using Learnup.Domain.AggregateRoots.AudioBooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class AudioBooksConfiguration : IEntityTypeConfiguration<AudioBooks>
{
    public void Configure(EntityTypeBuilder<AudioBooks> builder)
    {
        builder.ToTable("AudioBook");

        builder.HasKey(audioBook => audioBook.Id);

        builder.Property(audioBook => audioBook.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(audioBook => audioBook.Description)
            .HasMaxLength(2000);

        builder.Property(audioBook => audioBook.Author)
            .HasMaxLength(500);

        builder.Property(audioBook => audioBook.Level)
            .HasMaxLength(100);

        builder.Property(audioBook => audioBook.Year)
            .HasMaxLength(50);

        builder.Property(audioBook => audioBook.WordCount)
            .HasMaxLength(50);

        builder.Property(audioBook => audioBook.Source)
            .HasMaxLength(500);

        builder.Property(audioBook => audioBook.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(audioBook => audioBook.CoverId)
            .HasMaxLength(500);

        builder.Property(audioBook => audioBook.VoiceId)
            .HasMaxLength(500);

        builder.Property(audioBook => audioBook.TimingJsonId)
            .HasMaxLength(500);

        builder.HasMany(audioBook => audioBook.Items)
            .WithOne(item => item.AudioBook)
            .HasForeignKey(item => item.AudioBookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
