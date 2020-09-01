using Microsoft.EntityFrameworkCore.Migrations;

namespace JMS.Entity.Migrations
{
    public partial class RejectComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectComment",
                table: "Submission",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectComment",
                table: "Submission");
        }
    }
}
