using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class KeycloakIdColumnNameAndTypeUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "");
            
            migrationBuilder.Sql("""
                                 UPDATE public."User" 
                                 SET "IdentityId" = "KeycloakId"::text 
                                 """);

            
            migrationBuilder.DropColumn(
                name: "KeycloakId",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "User");

            migrationBuilder.Sql("""
                                 UPDATE public."User"
                                 SET "KeycloakId" = "IdentityId"::uuid
                                 WHERE "IdentityId" ~* '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$';
                                 """);

            migrationBuilder.AddColumn<Guid>(
                name: "KeycloakId",
                table: "User",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
