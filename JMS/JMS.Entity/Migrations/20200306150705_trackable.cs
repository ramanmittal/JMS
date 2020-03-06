using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JMS.Entity.Migrations
{
    public partial class trackable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Added",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Changed",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "Tenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Added",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Changed",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Tenants");
        }
    }
}
