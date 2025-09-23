using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleEventAccountingMobile.Migrations
{
    /// <inheritdoc />
    public partial class AddClientParticipation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsParticipate",
                table: "TrainingClients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscriber",
                table: "TrainingClients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsParticipate",
                table: "TrainingClients");

            migrationBuilder.DropColumn(
                name: "IsSubscriber",
                table: "TrainingClients");
        }
    }
}
