using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleEventAccountingMobile.Migrations
{
    /// <inheritdoc />
    public partial class AddFree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Free",
                table: "TrainingWalletHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Free",
                table: "TrainingWalletHistory");
        }
    }
}
