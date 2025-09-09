using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleEventAccountingMobile.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsAndGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientGroupBinding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientGroupId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGroupBinding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientGroupBinding_ClientGroup_ClientGroupId",
                        column: x => x.ClientGroupId,
                        principalTable: "ClientGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientGroupBinding_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientGroupBinding_ClientGroupId",
                table: "ClientGroupBinding",
                column: "ClientGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientGroupBinding_ClientId",
                table: "ClientGroupBinding",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientGroupBinding");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "ClientGroup");
        }
    }
}
