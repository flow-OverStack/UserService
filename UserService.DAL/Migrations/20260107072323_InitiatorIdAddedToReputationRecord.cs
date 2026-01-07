using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitiatorIdAddedToReputationRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReputationRecord_User_UserId",
                table: "ReputationRecord");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ReputationRecord",
                newName: "ReputationTargetId");

            migrationBuilder.RenameIndex(
                name: "IX_ReputationRecord_UserId",
                table: "ReputationRecord",
                newName: "IX_ReputationRecord_ReputationTargetId");

            migrationBuilder.AddColumn<long>(
                name: "InitiatorId",
                table: "ReputationRecord",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ReputationRecord_InitiatorId",
                table: "ReputationRecord",
                column: "InitiatorId");

            // Since there no way to track initiator, you can set any user from here
            const long userId = 12;
            migrationBuilder.Sql($"""
                                 UPDATE public."ReputationRecord"
                                 SET "InitiatorId" = {userId};
                                 """);

            migrationBuilder.AddForeignKey(
                name: "FK_ReputationRecord_User_InitiatorId",
                table: "ReputationRecord",
                column: "InitiatorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReputationRecord_User_ReputationTargetId",
                table: "ReputationRecord",
                column: "ReputationTargetId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReputationRecord_User_InitiatorId",
                table: "ReputationRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_ReputationRecord_User_ReputationTargetId",
                table: "ReputationRecord");

            migrationBuilder.DropIndex(
                name: "IX_ReputationRecord_InitiatorId",
                table: "ReputationRecord");

            migrationBuilder.DropColumn(
                name: "InitiatorId",
                table: "ReputationRecord");

            migrationBuilder.RenameColumn(
                name: "ReputationTargetId",
                table: "ReputationRecord",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ReputationRecord_ReputationTargetId",
                table: "ReputationRecord",
                newName: "IX_ReputationRecord_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReputationRecord_User_UserId",
                table: "ReputationRecord",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
