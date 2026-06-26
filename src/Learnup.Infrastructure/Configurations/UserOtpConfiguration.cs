using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserOtpConfiguration : IEntityTypeConfiguration<UserOtp>
{
    public void Configure(EntityTypeBuilder<UserOtp> builder)
    {
        builder.ToTable("UserOtp");

        builder.HasKey(otp => otp.Id);

        builder.Property(otp => otp.MobileNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(otp => otp.CodeHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(otp => otp.CreatedAt)
            .IsRequired();

        builder.Property(otp => otp.ExpiresAt)
            .IsRequired();

        builder.Property(otp => otp.ConsumedAt);

        builder.HasIndex(otp => new { otp.MobileNumber, otp.ConsumedAt });
    }
}
