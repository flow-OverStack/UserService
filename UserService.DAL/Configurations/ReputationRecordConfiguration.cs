using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.DAL.Configurations;

public class ReputationRecordConfiguration : IEntityTypeConfiguration<ReputationRecord>
{
    public void Configure(EntityTypeBuilder<ReputationRecord> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EntityId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Enabled).IsRequired().HasDefaultValue(true);
        builder.HasQueryFilter(x => x.Enabled);
    }
}