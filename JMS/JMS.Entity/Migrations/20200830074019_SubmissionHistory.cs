using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JMS.Entity.Migrations
{
    public partial class SubmissionHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubmissionHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubmissionId = table.Column<long>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    TenanatID = table.Column<long>(nullable: false),
                    ActorName = table.Column<string>(nullable: true),
                    ActorEmail = table.Column<string>(nullable: true),
                    ActionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionHistory_Tenants_TenanatID",
                        column: x => x.TenanatID,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionHistory_TenanatID",
                table: "SubmissionHistory",
                column: "TenanatID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionHistory");
        }
    }
}
