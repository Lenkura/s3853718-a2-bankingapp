using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class removeDuplicateDestinationAcc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_DestinationAccountAccountNumber",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_DestinationAccountAccountNumber",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DestinationAccountAccountNumber",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DestinationAccountNumber",
                table: "Transactions",
                column: "DestinationAccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_DestinationAccountNumber",
                table: "Transactions",
                column: "DestinationAccountNumber",
                principalTable: "Accounts",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_DestinationAccountNumber",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_DestinationAccountNumber",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "DestinationAccountAccountNumber",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DestinationAccountAccountNumber",
                table: "Transactions",
                column: "DestinationAccountAccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_DestinationAccountAccountNumber",
                table: "Transactions",
                column: "DestinationAccountAccountNumber",
                principalTable: "Accounts",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
