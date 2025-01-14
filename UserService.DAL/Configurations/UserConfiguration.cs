using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entity;

namespace UserService.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.KeycloakId).IsRequired();
        builder.Property(x => x.Username).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Password).IsRequired();
        builder.Property(x => x.Reputation).IsRequired().HasDefaultValue(0);

        //Email constraint
        builder.ToTable(t =>
            t.HasCheckConstraint("CK_User_Email", """
                                                  "Email" ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
                                                  """));
        //Reputation constraint
        builder.ToTable(t => t.HasCheckConstraint("CK_User_Reputation", """
                                                                        "Reputation" >= 0
                                                                        """));
        //Unique username and email
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();

        //relations
        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<UserRole>(
                x => x.HasOne<Role>().WithMany().HasForeignKey(y => y.RoleId),
                x => x.HasOne<User>().WithMany().HasForeignKey(y => y.UserId)
            );

        builder.HasMany(x => x.Badges)
            .WithMany(x => x.Users)
            .UsingEntity<UserBadge>(
                x => x.HasOne<Badge>().WithMany().HasForeignKey(y => y.BadgeId),
                x => x.HasOne<User>().WithMany().HasForeignKey(y => y.UserId)
            );

        builder.HasOne<UserToken>(x => x.UserToken)
            .WithOne(x => x.User)
            .HasForeignKey<UserToken>(x => x.UserId)
            .HasPrincipalKey<User>(x => x.Id);
    }
}