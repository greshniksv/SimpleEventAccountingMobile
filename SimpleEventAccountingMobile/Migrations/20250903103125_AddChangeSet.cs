using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleEventAccountingMobile.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "TrainingWallets");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "CashWallets");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ActionEvents");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TrainingWallets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Trainings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Clients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CashWallets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ActionEvents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventChangeSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Cash = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventChangeSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventChangeSets_ActionEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "ActionEvents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventChangeSets_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingChangeSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TrainingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Count = table.Column<decimal>(type: "TEXT", nullable: true),
                    Skip = table.Column<decimal>(type: "TEXT", nullable: true),
                    Free = table.Column<decimal>(type: "TEXT", nullable: true),
                    Subscription = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingChangeSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingChangeSets_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingChangeSets_Trainings_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "Trainings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventChangeSets_ClientId",
                table: "EventChangeSets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChangeSets_EventId",
                table: "EventChangeSets",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingChangeSets_ClientId",
                table: "TrainingChangeSets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingChangeSets_TrainingId",
                table: "TrainingChangeSets",
                column: "TrainingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventChangeSets");

            migrationBuilder.DropTable(
                name: "TrainingChangeSets");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TrainingWallets");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CashWallets");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ActionEvents");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "TrainingWallets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Trainings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "CashWallets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ActionEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
