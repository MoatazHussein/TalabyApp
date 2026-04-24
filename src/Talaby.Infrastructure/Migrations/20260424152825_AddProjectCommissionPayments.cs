using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talaby.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectCommissionPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProjectRequests",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProjectProposals",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProjectCommissionPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposalAmountSnapshot = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CommissionPercentage = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCommissionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectCommissionPayments_ProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "ProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectCommissionPayments_ProjectRequests_ProjectRequestId",
                        column: x => x.ProjectRequestId,
                        principalTable: "ProjectRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCommissionPaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectCommissionPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderName = table.Column<int>(type: "int", nullable: false),
                    ProviderChargeId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProviderTransactionReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderPaymentReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    RequestedCurrency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CheckoutUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCommissionPaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectCommissionPaymentAttempts_ProjectCommissionPayments_ProjectCommissionPaymentId",
                        column: x => x.ProjectCommissionPaymentId,
                        principalTable: "ProjectCommissionPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCommissionPaymentAttempts_IdempotencyKey",
                table: "ProjectCommissionPaymentAttempts",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCommissionPaymentAttempts_ProjectCommissionPaymentId",
                table: "ProjectCommissionPaymentAttempts",
                column: "ProjectCommissionPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCommissionPaymentAttempts_ProviderChargeId",
                table: "ProjectCommissionPaymentAttempts",
                column: "ProviderChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCommissionPaymentAttempts_ProviderTransactionReference",
                table: "ProjectCommissionPaymentAttempts",
                column: "ProviderTransactionReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCommissionPayments_ProjectProposalId",
                table: "ProjectCommissionPayments",
                column: "ProjectProposalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCommissionPayments_ProjectRequestId",
                table: "ProjectCommissionPayments",
                column: "ProjectRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectCommissionPaymentAttempts");

            migrationBuilder.DropTable(
                name: "ProjectCommissionPayments");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProjectRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProjectProposals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);
        }
    }
}
