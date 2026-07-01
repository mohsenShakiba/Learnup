using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class UserPlacementAnswerConfiguration : IEntityTypeConfiguration<UserPlacementAnswer>
{
    public void Configure(EntityTypeBuilder<UserPlacementAnswer> builder)
    {
        builder.ToTable("UserPlacementAnswer");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.PlacementQuestionId)
            .IsRequired();

        builder.Property(a => a.IsCorrect)
            .IsRequired();
    }
}
