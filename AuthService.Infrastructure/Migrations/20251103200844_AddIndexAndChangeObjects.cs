using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexAndChangeObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Roles_RoleId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Roles_RoleId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "Roles",
                newName: "Rolename");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username_Qnique",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Rolename_Unique",
                table: "Roles",
                column: "Rolename",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username_Qnique",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Rolename_Unique",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "Rolename",
                table: "Roles",
                newName: "RoleName");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Roles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleId",
                table: "Roles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Roles_RoleId",
                table: "Roles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
