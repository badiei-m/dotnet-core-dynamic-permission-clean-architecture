using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addParentToRoleAndPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parent_id",
                schema: "System",
                table: "role",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "parent_id",
                schema: "System",
                table: "permission",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_parent_id",
                schema: "System",
                table: "role",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_permission_parent_id",
                schema: "System",
                table: "permission",
                column: "parent_id");

            migrationBuilder.AddForeignKey(
                name: "FK_permission_permission_parent_id",
                schema: "System",
                table: "permission",
                column: "parent_id",
                principalSchema: "System",
                principalTable: "permission",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_role_role_parent_id",
                schema: "System",
                table: "role",
                column: "parent_id",
                principalSchema: "System",
                principalTable: "role",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_permission_permission_parent_id",
                schema: "System",
                table: "permission");

            migrationBuilder.DropForeignKey(
                name: "FK_role_role_parent_id",
                schema: "System",
                table: "role");

            migrationBuilder.DropIndex(
                name: "IX_role_parent_id",
                schema: "System",
                table: "role");

            migrationBuilder.DropIndex(
                name: "IX_permission_parent_id",
                schema: "System",
                table: "permission");

            migrationBuilder.DropColumn(
                name: "parent_id",
                schema: "System",
                table: "role");

            migrationBuilder.DropColumn(
                name: "parent_id",
                schema: "System",
                table: "permission");
        }
    }
}
