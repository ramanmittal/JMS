using Microsoft.EntityFrameworkCore.Migrations;

namespace JMS.Entity.Migrations
{
    public partial class submissionEditor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EditorId",
                table: "Submission",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_EditorId",
                table: "Submission",
                column: "EditorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Users_EditorId",
                table: "Submission",
                column: "EditorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Users_EditorId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_EditorId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "EditorId",
                table: "Submission");
        }
    }
}
