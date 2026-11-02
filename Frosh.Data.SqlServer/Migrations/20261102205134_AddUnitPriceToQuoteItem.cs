using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frosh.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitPriceToQuoteItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "QuoteItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "QuoteItems");
        }
    }
}
