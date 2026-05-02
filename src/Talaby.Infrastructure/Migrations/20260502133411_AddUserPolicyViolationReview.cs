using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talaby.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPolicyViolationReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewNote",
                table: "UserPolicyViolations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewStatus",
                table: "UserPolicyViolations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAtUtc",
                table: "UserPolicyViolations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedByUserId",
                table: "UserPolicyViolations",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewNote",
                table: "UserPolicyViolations");

            migrationBuilder.DropColumn(
                name: "ReviewStatus",
                table: "UserPolicyViolations");

            migrationBuilder.DropColumn(
                name: "ReviewedAtUtc",
                table: "UserPolicyViolations");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "UserPolicyViolations");
        }
    }
}
