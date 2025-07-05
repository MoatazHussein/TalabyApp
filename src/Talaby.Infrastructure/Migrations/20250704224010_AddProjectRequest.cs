using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talaby.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StoreCategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRequests_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectRequests_StoreCategories_StoreCategoryId",
                        column: x => x.StoreCategoryId,
                        principalTable: "StoreCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRequests_CreatorId",
                table: "ProjectRequests",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRequests_StoreCategoryId",
                table: "ProjectRequests",
                column: "StoreCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectRequests");
        }
    }
}
