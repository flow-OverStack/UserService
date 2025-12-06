using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UserService.Domain.Enums;

#nullable disable

namespace UserService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReputationRecordsAndReputationRulesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Reputation",
                table: "User");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_ReputationEarnedToday",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Reputation",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ReputationEarnedToday",
                table: "User");

            migrationBuilder.CreateTable(
                name: "ReputationRule",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: true),
                    ReputationChange = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReputationRule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReputationRecord",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ReputationRuleId = table.Column<long>(type: "bigint", nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReputationRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReputationRecord_ReputationRule_ReputationRuleId",
                        column: x => x.ReputationRuleId,
                        principalTable: "ReputationRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReputationRecord_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReputationRecord_ReputationRuleId",
                table: "ReputationRecord",
                column: "ReputationRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationRecord_UserId",
                table: "ReputationRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationRule_EventType",
                table: "ReputationRule",
                column: "EventType",
                unique: true);
            
            migrationBuilder.InsertData("ReputationRule", new[] { "EventType", "EntityType", "Group", "ReputationChange" }, new object[,]
            {
                { nameof(BaseEventType.AnswerAccepted), nameof(EntityType.Answer), null, 15 },
                { nameof(BaseEventType.DownvoteGivenForAnswer), nameof(EntityType.Answer), null, -1 },
                { nameof(BaseEventType.AnswerDownvote), nameof(EntityType.Answer), "AnswerVote", -2 },
                { nameof(BaseEventType.AnswerUpvote), nameof(EntityType.Answer), "AnswerVote", 10 },
                { nameof(BaseEventType.UserAcceptedAnswer), nameof(EntityType.Answer), null, 2 },
                { nameof(BaseEventType.QuestionDownvote), nameof(EntityType.Question), "QuestionVote", -2 },
                { nameof(BaseEventType.QuestionUpvote), nameof(EntityType.Question), "QuestionVote", 10 }
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReputationRecord");

            migrationBuilder.DropTable(
                name: "ReputationRule");

            migrationBuilder.AddColumn<int>(
                name: "Reputation",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "ReputationEarnedToday",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Reputation",
                table: "User",
                sql: "\"Reputation\" >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_ReputationEarnedToday",
                table: "User",
                sql: "\"ReputationEarnedToday\" >= 0 AND \"ReputationEarnedToday\" <= 200");
        }
    }
}
