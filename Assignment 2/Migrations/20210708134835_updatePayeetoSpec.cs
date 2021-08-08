using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcMCBA.Migrations
{
    public partial class updatePayeetoSpec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillPays_Accounts_AccountNumber1",
                table: "BillPays");

            migrationBuilder.DropIndex(
                name: "IX_BillPays_AccountNumber1",
                table: "BillPays");

            migrationBuilder.DropColumn(
                name: "AccountNumber1",
                table: "BillPays");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountNumber1",
                table: "BillPays",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillPays_AccountNumber1",
                table: "BillPays",
                column: "AccountNumber1");

            migrationBuilder.AddForeignKey(
                name: "FK_BillPays_Accounts_AccountNumber1",
                table: "BillPays",
                column: "AccountNumber1",
                principalTable: "Accounts",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
