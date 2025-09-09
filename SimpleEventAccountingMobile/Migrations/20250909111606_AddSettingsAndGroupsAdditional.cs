using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleEventAccountingMobile.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsAndGroupsAdditional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientGroupBinding_ClientGroup_ClientGroupId",
                table: "ClientGroupBinding");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientGroupBinding_Clients_ClientId",
                table: "ClientGroupBinding");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientGroupBinding",
                table: "ClientGroupBinding");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientGroup",
                table: "ClientGroup");

            migrationBuilder.RenameTable(
                name: "ClientGroupBinding",
                newName: "ClientGroupBindings");

            migrationBuilder.RenameTable(
                name: "ClientGroup",
                newName: "ClientGroups");

            migrationBuilder.RenameIndex(
                name: "IX_ClientGroupBinding_ClientId",
                table: "ClientGroupBindings",
                newName: "IX_ClientGroupBindings_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientGroupBinding_ClientGroupId",
                table: "ClientGroupBindings",
                newName: "IX_ClientGroupBindings_ClientGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientGroupBindings",
                table: "ClientGroupBindings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientGroups",
                table: "ClientGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientGroupBindings_ClientGroups_ClientGroupId",
                table: "ClientGroupBindings",
                column: "ClientGroupId",
                principalTable: "ClientGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientGroupBindings_Clients_ClientId",
                table: "ClientGroupBindings",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientGroupBindings_ClientGroups_ClientGroupId",
                table: "ClientGroupBindings");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientGroupBindings_Clients_ClientId",
                table: "ClientGroupBindings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientGroups",
                table: "ClientGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientGroupBindings",
                table: "ClientGroupBindings");

            migrationBuilder.RenameTable(
                name: "ClientGroups",
                newName: "ClientGroup");

            migrationBuilder.RenameTable(
                name: "ClientGroupBindings",
                newName: "ClientGroupBinding");

            migrationBuilder.RenameIndex(
                name: "IX_ClientGroupBindings_ClientId",
                table: "ClientGroupBinding",
                newName: "IX_ClientGroupBinding_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientGroupBindings_ClientGroupId",
                table: "ClientGroupBinding",
                newName: "IX_ClientGroupBinding_ClientGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientGroup",
                table: "ClientGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientGroupBinding",
                table: "ClientGroupBinding",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientGroupBinding_ClientGroup_ClientGroupId",
                table: "ClientGroupBinding",
                column: "ClientGroupId",
                principalTable: "ClientGroup",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientGroupBinding_Clients_ClientId",
                table: "ClientGroupBinding",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
