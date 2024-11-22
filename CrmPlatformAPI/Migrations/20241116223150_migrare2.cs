using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class migrare2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPhoto_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "CompanyPhoto");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPhoto_SoftwareCompanies_SoftwareCompanyId",
                table: "CompanyPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyPhoto",
                table: "CompanyPhoto");

            migrationBuilder.RenameTable(
                name: "CompanyPhoto",
                newName: "CompanyPhotos");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyPhoto_SoftwareCompanyId",
                table: "CompanyPhotos",
                newName: "IX_CompanyPhotos_SoftwareCompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyPhoto_BeneficiaryCompanyId",
                table: "CompanyPhotos",
                newName: "IX_CompanyPhotos_BeneficiaryCompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyPhotos",
                table: "CompanyPhotos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPhotos_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "CompanyPhotos",
                column: "BeneficiaryCompanyId",
                principalTable: "BeneficiaryCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPhotos_SoftwareCompanies_SoftwareCompanyId",
                table: "CompanyPhotos",
                column: "SoftwareCompanyId",
                principalTable: "SoftwareCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPhotos_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "CompanyPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPhotos_SoftwareCompanies_SoftwareCompanyId",
                table: "CompanyPhotos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyPhotos",
                table: "CompanyPhotos");

            migrationBuilder.RenameTable(
                name: "CompanyPhotos",
                newName: "CompanyPhoto");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyPhotos_SoftwareCompanyId",
                table: "CompanyPhoto",
                newName: "IX_CompanyPhoto_SoftwareCompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyPhotos_BeneficiaryCompanyId",
                table: "CompanyPhoto",
                newName: "IX_CompanyPhoto_BeneficiaryCompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyPhoto",
                table: "CompanyPhoto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPhoto_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "CompanyPhoto",
                column: "BeneficiaryCompanyId",
                principalTable: "BeneficiaryCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPhoto_SoftwareCompanies_SoftwareCompanyId",
                table: "CompanyPhoto",
                column: "SoftwareCompanyId",
                principalTable: "SoftwareCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
