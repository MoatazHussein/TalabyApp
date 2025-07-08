using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talaby.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToProjectRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ProjectRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ProjectRequests");
        }
    }
}
