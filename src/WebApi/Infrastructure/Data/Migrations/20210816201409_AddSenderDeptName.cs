using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Infrastructure.Data.Migrations
{
    public partial class AddSenderDeptName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Departments",
                table: "Notifications",
                newName: "ReceiverDepartments");

            migrationBuilder.AddColumn<int>(
                name: "SenderDepartment",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderDepartment",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "ReceiverDepartments",
                table: "Notifications",
                newName: "Departments");
        }
    }
}
