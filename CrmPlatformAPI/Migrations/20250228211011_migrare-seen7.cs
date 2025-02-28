using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class migrareseen7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Seen",
                table: "TicketStatusHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Seen",
                table: "TicketStatusHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
