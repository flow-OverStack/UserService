using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entity.Events;

namespace UserService.DAL.Configurations;

public class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
    {
        builder.HasKey(x => x.EventId);
        builder.Property(x => x.ProcessedAt).IsRequired();
    }
}