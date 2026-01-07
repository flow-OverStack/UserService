using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexRemovedFromReputationRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReputationRecord_UserId_EntityId_ReputationRuleId_Enabled",
                table: "ReputationRecord");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationRecord_UserId",
                table: "ReputationRecord",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReputationRecord_UserId",
                table: "ReputationRecord");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationRecord_UserId_EntityId_ReputationRuleId_Enabled",
                table: "ReputationRecord",
                columns: new[] { "UserId", "EntityId", "ReputationRuleId", "Enabled" },
                unique: true);
        }
    }
}
