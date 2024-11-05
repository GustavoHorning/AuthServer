using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthServer.Migrations
{
    /// <inheritdoc />
    public partial class NomeDaMigracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_RoleId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_UserId",
                table: "UserRole");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Roles_RoleId",
                table: "UserRole",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Long",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Users_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Roles_RoleId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Users_UserId",
                table: "UserRole");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Long",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
