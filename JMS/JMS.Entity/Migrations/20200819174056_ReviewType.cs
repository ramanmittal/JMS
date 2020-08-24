using Microsoft.EntityFrameworkCore.Migrations;

namespace JMS.Entity.Migrations
{
    public partial class ReviewType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewerType",
                table: "ReviewRequest");

            migrationBuilder.AddColumn<int>(
                name: "ReviewType",
                table: "ReviewRequest",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewType",
                table: "ReviewRequest");

            migrationBuilder.AddColumn<int>(
                name: "ReviewerType",
                table: "ReviewRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
