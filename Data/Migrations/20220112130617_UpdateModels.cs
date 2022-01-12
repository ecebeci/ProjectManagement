using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagement.Migrations.Projects
{
    public partial class UpdateModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Board",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "Board",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Template",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Template", x => x.TemplateId);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Board_Template_TemplateId",
                table: "Board");

            migrationBuilder.DropTable(
                name: "Template");

            migrationBuilder.DropIndex(
                name: "IX_Board_TemplateId",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Board");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Board",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
