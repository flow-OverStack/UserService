using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entity;

namespace UserService.DAL.Configurations;

public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.BadgeType).IsRequired().HasConversion<string>();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Description).IsRequired();
    }
}