using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auction_Website.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedSellerNameToAuctionEntityInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SellerName",
                table: "Auctions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellerName",
                table: "Auctions");
        }
    }
}
