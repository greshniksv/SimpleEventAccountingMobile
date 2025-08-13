using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleEventAccountingMobile.Migrations
{
    /// <inheritdoc />
    public partial class ChangeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashWalletHistory_Clients_ClientId",
                table: "CashWalletHistory");

            migrationBuilder.AlterColumn<Guid>(
                name: "TrainingId",
                table: "TrainingWalletHistory",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "CashWalletHistory",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_CashWalletHistory_Clients_ClientId",
                table: "CashWalletHistory",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashWalletHistory_Clients_ClientId",
                table: "CashWalletHistory");

            migrationBuilder.AlterColumn<Guid>(
                name: "TrainingId",
                table: "TrainingWalletHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "CashWalletHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CashWalletHistory_Clients_ClientId",
                table: "CashWalletHistory",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
