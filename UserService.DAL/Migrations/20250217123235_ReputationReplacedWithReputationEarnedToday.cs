using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReputationReplacedWithReputationEarnedToday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_User_ReputationEarnedToday",
                table: "User");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_ReputationEarnedToday",
                table: "User",
                sql: "\"ReputationEarnedToday\" >= 0 AND \"ReputationEarnedToday\" <= 200");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_User_ReputationEarnedToday",
                table: "User");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_ReputationEarnedToday",
                table: "User",
                sql: "\"Reputation\" >= 0 AND \"Reputation\" <= 200");
        }
    }
}
