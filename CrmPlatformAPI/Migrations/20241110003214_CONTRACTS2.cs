using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class CONTRACTS2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_SoftwareCompanies_SoftwareCompanyId",
                table: "Contract");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contract",
                table: "Contract");

            migrationBuilder.RenameTable(
                name: "Contract",
                newName: "Contracts");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_SoftwareCompanyId",
                table: "Contracts",
                newName: "IX_Contracts_SoftwareCompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_BeneficiaryCompanyId",
                table: "Contracts",
                newName: "IX_Contracts_BeneficiaryCompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "Contracts",
                column: "BeneficiaryCompanyId",
                principalTable: "BeneficiaryCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_SoftwareCompanies_SoftwareCompanyId",
                table: "Contracts",
                column: "SoftwareCompanyId",
                principalTable: "SoftwareCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_SoftwareCompanies_SoftwareCompanyId",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.RenameTable(
                name: "Contracts",
                newName: "Contract");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_SoftwareCompanyId",
                table: "Contract",
                newName: "IX_Contract_SoftwareCompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_BeneficiaryCompanyId",
                table: "Contract",
                newName: "IX_Contract_BeneficiaryCompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contract",
                table: "Contract",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_BeneficiaryCompanies_BeneficiaryCompanyId",
                table: "Contract",
                column: "BeneficiaryCompanyId",
                principalTable: "BeneficiaryCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_SoftwareCompanies_SoftwareCompanyId",
                table: "Contract",
                column: "SoftwareCompanyId",
                principalTable: "SoftwareCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
