using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    /// <inheritdoc />
    public partial class notificationChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromUserId",
                table: "Notifications",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_FromUserId",
                table: "Notifications",
                column: "FromUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_FromUserId",
                table: "Notifications",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_FromUserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_FromUserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "Notifications");
        }
    }
}
