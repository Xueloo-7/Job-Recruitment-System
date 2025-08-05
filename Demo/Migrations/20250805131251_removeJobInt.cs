using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    /// <inheritdoc />
    public partial class removeJobInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Jobs_JobId1",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_JobId1",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "JobId1",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "JobId",
                table: "Applications",
                type: "nvarchar(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobId",
                table: "Applications",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_JobId",
                table: "Applications");

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "Applications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)");

            migrationBuilder.AddColumn<string>(
                name: "JobId1",
                table: "Applications",
                type: "nvarchar(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobId1",
                table: "Applications",
                column: "JobId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Jobs_JobId1",
                table: "Applications",
                column: "JobId1",
                principalTable: "Jobs",
                principalColumn: "Id");
        }
    }
}
