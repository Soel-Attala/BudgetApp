using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuestos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClientName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ClientEmail = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ClientPhone = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ClientAddress = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DollarRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    BenefitPercentage = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    IncludeFees = table.Column<bool>(type: "INTEGER", nullable: false),
                    FeesCost = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    IncludeLabor = table.Column<bool>(type: "INTEGER", nullable: false),
                    LaborCost = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Taxes_IncludeTaxes = table.Column<bool>(type: "INTEGER", nullable: false),
                    Taxes_IVA = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Taxes_IVAT = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Taxes_IB = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Taxes_IG = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Taxes_IC = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Taxes_BankFees = table.Column<decimal>(type: "decimal(9,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BaseCurrency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    QuoteCurrency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LaborCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    PriceUSD = table.Column<decimal>(type: "decimal(18,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subgroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subgroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    MaterialId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MaterialName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    PriceUSD = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    SubgroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SubgroupName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    LaborItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LaborItemName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    PriceARS = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    LaborQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    BudgetId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetItems_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaborItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PriceARS = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaborItems_LaborCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "LaborCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SubgroupMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubgroupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaterialId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaterialName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    PriceUSD = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubgroupMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubgroupMaterials_Subgroups_SubgroupId",
                        column: x => x.SubgroupId,
                        principalTable: "Subgroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetId",
                table: "BudgetItems",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_Type",
                table: "BudgetItems",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Date",
                table: "Budgets",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Name",
                table: "Budgets",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_BaseCurrency_QuoteCurrency",
                table: "ExchangeRates",
                columns: new[] { "BaseCurrency", "QuoteCurrency" });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_TimestampUtc",
                table: "ExchangeRates",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_LaborItems_CategoryId",
                table: "LaborItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubgroupMaterials_SubgroupId",
                table: "SubgroupMaterials",
                column: "SubgroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetItems");

            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "LaborItems");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "SubgroupMaterials");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "LaborCategories");

            migrationBuilder.DropTable(
                name: "Subgroups");
        }
    }
}
