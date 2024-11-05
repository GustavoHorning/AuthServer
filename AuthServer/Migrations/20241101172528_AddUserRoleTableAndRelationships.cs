using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthServer.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleTableAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserRole",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RoleUser",
                columns: table => new
                {
                    RolesLong = table.Column<int>(type: "int", nullable: false),
                    UsersID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesLong, x.UsersID });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesLong",
                        column: x => x.RolesLong,
                        principalTable: "Roles",
                        principalColumn: "Long",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersID",
                        column: x => x.UsersID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersID",
                table: "RoleUser",
                column: "UsersID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" });
        }
    }
}
