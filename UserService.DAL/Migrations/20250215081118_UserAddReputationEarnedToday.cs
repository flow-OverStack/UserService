using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UserAddReputationEarnedToday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReputationEarnedToday",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_ReputationEarnedToday",
                table: "User",
                sql: "\"Reputation\" >= 0 AND \"Reputation\" <= 200");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_User_ReputationEarnedToday",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ReputationEarnedToday",
                table: "User");
        }
    }
}
