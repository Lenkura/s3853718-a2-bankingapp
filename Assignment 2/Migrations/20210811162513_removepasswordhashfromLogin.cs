using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcMCBA.Migrations
{
    public partial class removepasswordhashfromLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CH_Login_PasswordHash",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Logins");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Logins",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CH_Login_PasswordHash",
                table: "Logins",
                sql: "len(PasswordHash) = 64");
        }
    }
}
