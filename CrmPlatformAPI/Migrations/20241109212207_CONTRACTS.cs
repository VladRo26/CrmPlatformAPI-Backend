using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class CONTRACTS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BeneficiaryCompanyId = table.Column<int>(type: "int", nullable: false),
                    SoftwareCompanyId = table.Column<int>(type: "int", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedFinishDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OffersSupport = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contract_BeneficiaryCompanies_BeneficiaryCompanyId",
                        column: x => x.BeneficiaryCompanyId,
                        principalTable: "BeneficiaryCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contract_SoftwareCompanies_SoftwareCompanyId",
                        column: x => x.SoftwareCompanyId,
                        principalTable: "SoftwareCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contract_BeneficiaryCompanyId",
                table: "Contract",
                column: "BeneficiaryCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_SoftwareCompanyId",
                table: "Contract",
                column: "SoftwareCompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contract");
        }
    }
}
