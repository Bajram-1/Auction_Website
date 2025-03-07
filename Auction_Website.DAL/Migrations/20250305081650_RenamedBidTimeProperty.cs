using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auction_Website.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenamedBidTimeProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BidTime",
                table: "Bids",
                newName: "TimePlaced");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimePlaced",
                table: "Bids",
                newName: "BidTime");
        }
    }
}
