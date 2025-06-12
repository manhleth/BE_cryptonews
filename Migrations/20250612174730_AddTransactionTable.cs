using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPaper.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TransactionHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FromAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromToken = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ToToken = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(28,18)", precision: 28, scale: 18, nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(28,18)", precision: 28, scale: 18, nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "SEND"),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "PENDING"),
                    GasUsed = table.Column<decimal>(type: "decimal(28,18)", precision: 28, scale: 18, nullable: false),
                    GasPrice = table.Column<decimal>(type: "decimal(28,18)", precision: 28, scale: 18, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionHash",
                table: "Transactions",
                column: "TransactionHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
