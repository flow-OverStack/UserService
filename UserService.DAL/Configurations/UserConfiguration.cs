using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.KeycloakId).IsRequired();
        builder.Property(x => x.Username).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(255);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.LastLoginAt);
        builder.Property(x => x.ReputationEarnedToday).IsRequired().HasDefaultValue(0);

        //Email constraint
        builder.ToTable(t =>
            t.HasCheckConstraint("CK_User_Email", """
                                                  "Email" ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
                                                  """));

        //Username constraint
        builder.ToTable(t => t.HasCheckConstraint("CK_User_Username_LowerCase", """
            "Username" = LOWER("Username")
            """));
        //Unique username and email
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.LastLoginAt);

        //relations
        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<UserRole>(
                x => x.HasOne<Role>().WithMany().HasForeignKey(y => y.RoleId),
                x => x.HasOne<User>().WithMany().HasForeignKey(y => y.UserId)
            );
    }
}