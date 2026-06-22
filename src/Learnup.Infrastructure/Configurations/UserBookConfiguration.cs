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
        
        builder.Property(ub => ub.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ub => ub.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ub => ub.CurrentRef)
            .HasMaxLength(500);

        builder.Property(ub => ub.UploadedAt)
            .IsRequired();

        builder.HasOne(ub => ub.User)
            .WithMany(u => u.Books)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
