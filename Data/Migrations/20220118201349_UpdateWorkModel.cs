using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagement.Migrations.Projects
{
    public partial class UpdateWorkModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Work_Member_MemberId",
                table: "Work");

            migrationBuilder.DropIndex(
                name: "IX_Work_MemberId",
                table: "Work");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Work");

            migrationBuilder.CreateTable(
                name: "AddTemplateViewModel",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_AddTemplateViewModel_Template_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Template",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberWork",
                columns: table => new
                {
                    MembersMemberId = table.Column<int>(type: "int", nullable: false),
                    WorksWorkId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberWork", x => new { x.MembersMemberId, x.WorksWorkId });
                    table.ForeignKey(
                        name: "FK_MemberWork_Member_MembersMemberId",
                        column: x => x.MembersMemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberWork_Work_WorksWorkId",
                        column: x => x.WorksWorkId,
                        principalTable: "Work",
                        principalColumn: "WorkId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddTemplateViewModel_TemplateId",
                table: "AddTemplateViewModel",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberWork_WorksWorkId",
                table: "MemberWork",
                column: "WorksWorkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddTemplateViewModel");

            migrationBuilder.DropTable(
                name: "MemberWork");

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Work",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Work_MemberId",
                table: "Work",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Work_Member_MemberId",
                table: "Work",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
