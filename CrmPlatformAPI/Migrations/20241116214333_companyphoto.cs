using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class companyphoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyPhoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeneficiaryCompanyId = table.Column<int>(type: "int", nullable: true),
                    SoftwareCompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyPhoto_BeneficiaryCompanies_BeneficiaryCompanyId",
                        column: x => x.BeneficiaryCompanyId,
                        principalTable: "BeneficiaryCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPhoto_SoftwareCompanies_SoftwareCompanyId",
                        column: x => x.SoftwareCompanyId,
                        principalTable: "SoftwareCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPhoto_BeneficiaryCompanyId",
                table: "CompanyPhoto",
                column: "BeneficiaryCompanyId",
                unique: true,
                filter: "[BeneficiaryCompanyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPhoto_SoftwareCompanyId",
                table: "CompanyPhoto",
                column: "SoftwareCompanyId",
                unique: true,
                filter: "[SoftwareCompanyId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyPhoto");
        }
    }
}
