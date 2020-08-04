using Microsoft.EntityFrameworkCore.Migrations;

namespace JMS.Entity.Migrations
{
    public partial class cascadefalse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Users_Id",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Contributors_Submission_SubmissionId",
                table: "Contributors");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalSettings_Tenants_TenantId",
                table: "JournalSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Users_UserID",
                table: "Submission");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmisssionFile_TenantArticleComponent_ArticleComponentId",
                table: "SubmisssionFile");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmisssionFile_Submission_SubmissionId",
                table: "SubmisssionFile");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Users_Id",
                table: "Authors",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contributors_Submission_SubmissionId",
                table: "Contributors",
                column: "SubmissionId",
                principalTable: "Submission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalSettings_Tenants_TenantId",
                table: "JournalSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Users_UserID",
                table: "Submission",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmisssionFile_TenantArticleComponent_ArticleComponentId",
                table: "SubmisssionFile",
                column: "ArticleComponentId",
                principalTable: "TenantArticleComponent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmisssionFile_Submission_SubmissionId",
                table: "SubmisssionFile",
                column: "SubmissionId",
                principalTable: "Submission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Users_Id",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Contributors_Submission_SubmissionId",
                table: "Contributors");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalSettings_Tenants_TenantId",
                table: "JournalSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Users_UserID",
                table: "Submission");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmisssionFile_TenantArticleComponent_ArticleComponentId",
                table: "SubmisssionFile");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmisssionFile_Submission_SubmissionId",
                table: "SubmisssionFile");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Users_Id",
                table: "Authors",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contributors_Submission_SubmissionId",
                table: "Contributors",
                column: "SubmissionId",
                principalTable: "Submission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalSettings_Tenants_TenantId",
                table: "JournalSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Users_UserID",
                table: "Submission",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmisssionFile_TenantArticleComponent_ArticleComponentId",
                table: "SubmisssionFile",
                column: "ArticleComponentId",
                principalTable: "TenantArticleComponent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmisssionFile_Submission_SubmissionId",
                table: "SubmisssionFile",
                column: "SubmissionId",
                principalTable: "Submission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
