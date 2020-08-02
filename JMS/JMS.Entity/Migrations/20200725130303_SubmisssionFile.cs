using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JMS.Entity.Migrations
{
    public partial class SubmisssionFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubmisssionFile",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileId = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    UploadedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Creator = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    SubmissionId = table.Column<long>(nullable: false),
                    ArticleComponentId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmisssionFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmisssionFile_TenantArticleComponent_ArticleComponentId",
                        column: x => x.ArticleComponentId,
                        principalTable: "TenantArticleComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmisssionFile_Submission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmisssionFile_ArticleComponentId",
                table: "SubmisssionFile",
                column: "ArticleComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmisssionFile_SubmissionId",
                table: "SubmisssionFile",
                column: "SubmissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmisssionFile");
        }
    }
}
