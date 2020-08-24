using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JMS.Entity.Migrations
{
    public partial class ReviewRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewRequest",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewerID = table.Column<long>(nullable: false),
                    SubmissionId = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: true),
                    ReviewerType = table.Column<int>(nullable: false),
                    EditorComment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewRequest", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReviewRequest_Users_ReviewerID",
                        column: x => x.ReviewerID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewRequest_Submission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRequest_ReviewerID",
                table: "ReviewRequest",
                column: "ReviewerID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRequest_SubmissionId",
                table: "ReviewRequest",
                column: "SubmissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewRequest");
        }
    }
}
