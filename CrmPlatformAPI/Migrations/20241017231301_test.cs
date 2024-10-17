using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SoftwareCompanies_SoftwareCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SoftwareCompanyId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "SoftwareCompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SoftwareCompanyId",
                table: "AspNetUsers",
                column: "SoftwareCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SoftwareCompanies_SoftwareCompanyId",
                table: "AspNetUsers",
                column: "SoftwareCompanyId",
                principalTable: "SoftwareCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SoftwareCompanies_SoftwareCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SoftwareCompanyId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "SoftwareCompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SoftwareCompanyId",
                table: "AspNetUsers",
                column: "SoftwareCompanyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SoftwareCompanies_SoftwareCompanyId",
                table: "AspNetUsers",
                column: "SoftwareCompanyId",
                principalTable: "SoftwareCompanies",
                principalColumn: "Id");
        }
    }
}
