using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleEventAccountingMobile.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Birthday = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashWalletHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Cash = table.Column<decimal>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashWalletHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashWalletHistory_ActionEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "ActionEvents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CashWalletHistory_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashWallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Cash = table.Column<decimal>(type: "TEXT", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashWallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashWallets_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventClients_ActionEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "ActionEvents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingWallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Count = table.Column<decimal>(type: "TEXT", nullable: false),
                    Skip = table.Column<decimal>(type: "TEXT", nullable: false),
                    Free = table.Column<decimal>(type: "TEXT", nullable: false),
                    Subscription = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingWallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingWallets_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TrainingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingClients_Trainings_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "Trainings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingWalletHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TrainingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Count = table.Column<decimal>(type: "TEXT", nullable: false),
                    Skip = table.Column<decimal>(type: "TEXT", nullable: false),
                    Subscription = table.Column<bool>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingWalletHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingWalletHistory_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingWalletHistory_Trainings_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "Trainings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashWalletHistory_ClientId",
                table: "CashWalletHistory",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CashWalletHistory_EventId",
                table: "CashWalletHistory",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_CashWallets_ClientId",
                table: "CashWallets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EventClients_ClientId",
                table: "EventClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EventClients_EventId",
                table: "EventClients",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingClients_ClientId",
                table: "TrainingClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingClients_TrainingId",
                table: "TrainingClients",
                column: "TrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingWalletHistory_ClientId",
                table: "TrainingWalletHistory",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingWalletHistory_TrainingId",
                table: "TrainingWalletHistory",
                column: "TrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingWallets_ClientId",
                table: "TrainingWallets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingWallets_Subscription",
                table: "TrainingWallets",
                column: "Subscription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashWalletHistory");

            migrationBuilder.DropTable(
                name: "CashWallets");

            migrationBuilder.DropTable(
                name: "EventClients");

            migrationBuilder.DropTable(
                name: "TrainingClients");

            migrationBuilder.DropTable(
                name: "TrainingWalletHistory");

            migrationBuilder.DropTable(
                name: "TrainingWallets");

            migrationBuilder.DropTable(
                name: "ActionEvents");

            migrationBuilder.DropTable(
                name: "Trainings");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
