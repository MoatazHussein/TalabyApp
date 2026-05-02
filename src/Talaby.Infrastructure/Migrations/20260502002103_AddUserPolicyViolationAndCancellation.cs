using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talaby.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPolicyViolationAndCancellation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "ProjectRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAtUtc",
                table: "ProjectRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByUserId",
                table: "ProjectRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "ProjectProposals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAtUtc",
                table: "ProjectProposals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByUserId",
                table: "ProjectProposals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserPolicyViolations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPolicyViolations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPolicyViolations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPolicyViolations_ProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "ProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPolicyViolations_ProjectRequests_ProjectRequestId",
                        column: x => x.ProjectRequestId,
                        principalTable: "ProjectRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPolicyViolations_ProjectProposalId",
                table: "UserPolicyViolations",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPolicyViolations_ProjectRequestId",
                table: "UserPolicyViolations",
                column: "ProjectRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPolicyViolations_UserId_ProjectRequestId_Reason",
                table: "UserPolicyViolations",
                columns: new[] { "UserId", "ProjectRequestId", "Reason" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPolicyViolations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "ProjectRequests");

            migrationBuilder.DropColumn(
                name: "CancelledAtUtc",
                table: "ProjectRequests");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "ProjectRequests");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "ProjectProposals");

            migrationBuilder.DropColumn(
                name: "CancelledAtUtc",
                table: "ProjectProposals");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "ProjectProposals");
        }
    }
}
