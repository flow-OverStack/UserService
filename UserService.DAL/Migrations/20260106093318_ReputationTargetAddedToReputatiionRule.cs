using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReputationTargetAddedToReputatiionRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReputationRule_EventType",
                table: "ReputationRule");

            migrationBuilder.AddColumn<int>(
                name: "ReputationTarget",
                table: "ReputationRule",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReputationRule_EventType_EntityType_ReputationTarget",
                table: "ReputationRule",
                columns: new[] { "EventType", "EntityType", "ReputationTarget" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ReputationRule_ReputationTarget_Enum",
                table: "ReputationRule",
                sql: "\"ReputationTarget\" IN (0,1)");

            migrationBuilder.Sql("""
                                 -- EntityDownvoted / Answer
                                 UPDATE public."ReputationRule"
                                 SET "EventType" = 'EntityDownvoted',
                                     "EntityType" = 'Answer',
                                     "Group" = 'Vote',
                                     "ReputationChange" = -1,
                                     "ReputationTarget" = 1
                                 WHERE "Id" = 2;
                                 
                                 UPDATE public."ReputationRule"
                                 SET "EventType" = 'EntityDownvoted',
                                     "EntityType" = 'Answer',
                                     "Group" = 'Vote',
                                     "ReputationChange" = -2,
                                     "ReputationTarget" = 0
                                 WHERE "Id" = 3;
                                 
                                 -- EntityUpvoted / Answer
                                 UPDATE public."ReputationRule"
                                 SET "EventType" = 'EntityUpvoted',
                                     "EntityType" = 'Answer',
                                     "Group" = 'Vote',
                                     "ReputationChange" = 10,
                                     "ReputationTarget" = 0
                                 WHERE "Id" = 4;
                                 
                                 -- EntityAccepted / Answer
                                 UPDATE public."ReputationRule"
                                 SET "EventType" = 'EntityAccepted',
                                     "EntityType" = 'Answer',
                                     "Group" = NULL,
                                     "ReputationChange" = 2,
                                     "ReputationTarget" = 1
                                 WHERE "Id" = 5;
                                 
                                 UPDATE public."ReputationRule"
                                 SET "EventType" = 'EntityAccepted',
                                     "EntityType" = 'Answer',
                                     "Group" = NULL,
                                     "ReputationChange" = 15,
                                     "ReputationTarget" = 0
                                 WHERE "Id" = 1;
                                 
                                 -- EntityDownvoted / Question
                                 UPDATE public."ReputationRule"
                                 SET "EventType" = 'EntityDownvoted',
                                     "EntityType" = 'Question',
                                     "Group" = 'Vote',
                                     "ReputationChange" = -2,
                                     "ReputationTarget" = 0
                                 WHERE "Id" = 6;
                                 
                                 -- EntityUpvoted / Question
                                 UPDATE public."ReputationRule"
                                 SET "EventType" = 'EntityUpvoted',
                                     "EntityType" = 'Question',
                                     "Group" = 'Vote',
                                     "ReputationChange" = 10,
                                     "ReputationTarget" = 0
                                 WHERE "Id" = 7;
                                 
                                 """);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReputationRule_EventType_EntityType_ReputationTarget",
                table: "ReputationRule");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ReputationRule_ReputationTarget_Enum",
                table: "ReputationRule");

            migrationBuilder.DropColumn(
                name: "ReputationTarget",
                table: "ReputationRule");
            
            migrationBuilder.Sql("""
                                     -- Revert to original values before migration

                                     UPDATE public."ReputationRule"
                                     SET "EventType" = 'DownvoteGivenForAnswer',
                                         "EntityType" = 'Answer',
                                         "Group" = NULL,
                                         "ReputationChange" = -1
                                     WHERE "Id" = 2;

                                     UPDATE public."ReputationRule"
                                     SET "EventType" = 'AnswerDownvote',
                                         "EntityType" = 'Answer',
                                         "Group" = 'AnswerVote',
                                         "ReputationChange" = -2
                                     WHERE "Id" = 3;

                                     UPDATE public."ReputationRule"
                                     SET "EventType" = 'AnswerUpvote',
                                         "EntityType" = 'Answer',
                                         "Group" = 'AnswerVote',
                                         "ReputationChange" = 10
                                     WHERE "Id" = 4;

                                     UPDATE public."ReputationRule"
                                     SET "EventType" = 'UserAcceptedAnswer',
                                         "EntityType" = 'Answer',
                                         "Group" = NULL,
                                         "ReputationChange" = 2
                                     WHERE "Id" = 5;

                                     UPDATE public."ReputationRule"
                                     SET "EventType" = 'AnswerAccepted',
                                         "EntityType" = 'Answer',
                                         "Group" = NULL,
                                         "ReputationChange" = 15
                                     WHERE "Id" = 1;

                                     UPDATE public."ReputationRule"
                                     SET "EventType" = 'QuestionDownvote',
                                         "EntityType" = 'Question',
                                         "Group" = 'QuestionVote',
                                         "ReputationChange" = -2
                                     WHERE "Id" = 6;

                                     UPDATE public."ReputationRule"
                                     SET "EventType" = 'QuestionUpvote',
                                         "EntityType" = 'Question',
                                         "Group" = 'QuestionVote',
                                         "ReputationChange" = 10
                                     WHERE "Id" = 7;
                                 """);

            migrationBuilder.CreateIndex(
                name: "IX_ReputationRule_EventType",
                table: "ReputationRule",
                column: "EventType",
                unique: true);
        }
    }
}
