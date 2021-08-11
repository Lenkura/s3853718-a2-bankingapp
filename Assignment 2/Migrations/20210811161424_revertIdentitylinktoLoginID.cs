using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcMCBA.Migrations
{
    public partial class revertIdentitylinktoLoginID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Customers_CustomerID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CustomerID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "LoginID",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LoginID",
                table: "AspNetUsers",
                column: "LoginID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Logins_LoginID",
                table: "AspNetUsers",
                column: "LoginID",
                principalTable: "Logins",
                principalColumn: "LoginID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Logins_LoginID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LoginID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LoginID",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CustomerID",
                table: "AspNetUsers",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Customers_CustomerID",
                table: "AspNetUsers",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
