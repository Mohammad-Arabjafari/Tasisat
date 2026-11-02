using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frosh.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSpecificationToQuoteItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductSpecification",
                table: "QuoteItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSpecification",
                table: "QuoteItems");
        }
    }
}
