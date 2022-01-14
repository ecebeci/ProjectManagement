using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagement.Migrations.Projects
{
    public partial class UpdateTemplateModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Board_Template_TemplateId",
                table: "Board");

            migrationBuilder.DropIndex(
                name: "IX_Board_TemplateId",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Board");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "Board",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Board_TemplateId",
                table: "Board",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Board_Template_TemplateId",
                table: "Board",
                column: "TemplateId",
                principalTable: "Template",
                principalColumn: "TemplateId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
