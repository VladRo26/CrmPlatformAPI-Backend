using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class benefcompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BeneficiaryCompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BeneficiaryCompanyId",
                table: "AspNetUsers",
                column: "BeneficiaryCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "AspNetUsers",
                column: "BeneficiaryCompanyId",
                principalTable: "BeneficiaryCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BeneficiaryCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BeneficiaryCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "AspNetUsers");
        }
    }
}
