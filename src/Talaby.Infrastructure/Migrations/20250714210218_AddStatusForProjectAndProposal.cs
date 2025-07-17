using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talaby.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusForProjectAndProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProjectRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProjectProposals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProjectRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProjectProposals");
        }
    }
}
