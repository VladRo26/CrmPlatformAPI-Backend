using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class bufix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SoftwareCompany_SoftwareCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SoftwareCompany",
                table: "SoftwareCompany");

            migrationBuilder.RenameTable(
                name: "SoftwareCompany",
                newName: "SoftwareCompanies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SoftwareCompanies",
                table: "SoftwareCompanies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SoftwareCompanies_SoftwareCompanyId",
                table: "AspNetUsers",
                column: "SoftwareCompanyId",
                principalTable: "SoftwareCompanies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SoftwareCompanies_SoftwareCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SoftwareCompanies",
                table: "SoftwareCompanies");

            migrationBuilder.RenameTable(
                name: "SoftwareCompanies",
                newName: "SoftwareCompany");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SoftwareCompany",
                table: "SoftwareCompany",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SoftwareCompany_SoftwareCompanyId",
                table: "AspNetUsers",
                column: "SoftwareCompanyId",
                principalTable: "SoftwareCompany",
                principalColumn: "Id");
        }
    }
}
