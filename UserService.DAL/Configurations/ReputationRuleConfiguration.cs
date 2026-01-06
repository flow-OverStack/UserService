using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

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
        builder.Property(x => x.ReputationTarget).HasConversion<int>().IsRequired()
            .HasDefaultValue(ReputationTarget.Author);

        var allowedTargets = string.Join(',', Enum.GetValues<ReputationTarget>().Select(x => (int)x));
        builder.ToTable(t => t.HasCheckConstraint("CK_ReputationRule_ReputationTarget_Enum", $"""
             "{nameof(ReputationRule.ReputationTarget)}" IN ({allowedTargets})
             """));

        builder.HasMany(x => x.ReputationRecords)
            .WithOne(x => x.ReputationRule)
            .HasForeignKey(x => x.ReputationRuleId)
            .HasPrincipalKey(x => x.Id);

        builder.HasIndex(x => new { x.EventType, x.EntityType, x.ReputationTarget }).IsUnique();

        // Domain rule: for a given EventType + EntityType, Group is either NULL or identical across all ReputationRule records.
    }
}