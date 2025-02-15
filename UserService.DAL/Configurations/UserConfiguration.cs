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
        builder.Property(x => x.Reputation).IsRequired().HasDefaultValue(1);
        builder.Property(x => x.ReputationEarnedToday).IsRequired().HasDefaultValue(0);

        //Email constraint
        builder.ToTable(t =>
            t.HasCheckConstraint("CK_User_Email", """
                                                  "Email" ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
                                                  """));
        //Reputation constraint
        builder.ToTable(t => t.HasCheckConstraint("CK_User_Reputation", """
                                                                        "Reputation" >= 1
                                                                        """));

        //ReputationEarnedToday constraint
        builder.ToTable(t => t.HasCheckConstraint("CK_User_ReputationEarnedToday", $"""
             "Reputation" >= 0 AND "Reputation" <= {User.MaxDailyReputation}
             """));

        //Username constraint
        builder.ToTable(t => t.HasCheckConstraint("CK_User_Username_LowerCase", """
            "Username" = LOWER("Username")
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

        builder.HasOne<UserToken>(x => x.UserToken)
            .WithOne(x => x.User)
            .HasForeignKey<UserToken>(x => x.UserId)
            .HasPrincipalKey<User>(x => x.Id);
    }
}