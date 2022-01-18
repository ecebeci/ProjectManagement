using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagement.Migrations.Projects
{
    public partial class UpdateWorkMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberWork");

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Work",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkMember",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    WorkId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkMember", x => new { x.WorkId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_WorkMember_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkMember_Work_WorkId",
                        column: x => x.WorkId,
                        principalTable: "Work",
                        principalColumn: "WorkId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Work_MemberId",
                table: "Work",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkMember_MemberId",
                table: "WorkMember",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Work_Member_MemberId",
                table: "Work",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Work_Member_MemberId",
                table: "Work");

            migrationBuilder.DropTable(
                name: "WorkMember");

            migrationBuilder.DropIndex(
                name: "IX_Work_MemberId",
                table: "Work");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Work");

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
                name: "IX_MemberWork_WorksWorkId",
                table: "MemberWork",
                column: "WorksWorkId");
        }
    }
}
