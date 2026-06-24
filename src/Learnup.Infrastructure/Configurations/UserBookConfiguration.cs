using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserBookConfiguration : IEntityTypeConfiguration<UserBook>
{
    public void Configure(EntityTypeBuilder<UserBook> builder)
    {
        builder.ToTable("UserBook");

        builder.HasKey(ub => ub.Id);

        builder.Property(ub => ub.CurrentRef)
            .HasMaxLength(500);

        builder.Property(ub => ub.Progress);

        builder.Property(ub => ub.CreatedAt)
            .IsRequired();

        builder.HasIndex(ub => new { ub.UserId, ub.EbookId })
            .IsUnique();

        builder.HasOne(ub => ub.User)
            .WithMany(u => u.Books)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ub => ub.Ebook)
            .WithMany(ebook => ebook.Users)
            .HasForeignKey(ub => ub.EbookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
