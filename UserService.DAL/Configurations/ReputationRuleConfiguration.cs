using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.DAL.Configurations;

public class ReputationRuleConfiguration : IEntityTypeConfiguration<ReputationRule>
{
    public void Configure(EntityTypeBuilder<ReputationRule> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EventType).IsRequired();
        builder.Property(x => x.EntityType).IsRequired();
        builder.Property(x => x.ReputationChange).IsRequired();
        builder.Property(x => x.Group).IsRequired(false);

        builder.HasMany(x => x.ReputationRecords)
            .WithOne(x => x.ReputationRule)
            .HasForeignKey(x => x.ReputationRuleId)
            .HasPrincipalKey(x => x.Id);

        builder.HasIndex(x => x.EventType).IsUnique();
    }
}