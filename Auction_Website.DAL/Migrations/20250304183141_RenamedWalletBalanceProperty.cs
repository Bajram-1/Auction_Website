using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auction_Website.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenamedWalletBalanceProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Wallet",
                table: "Users",
                newName: "WalletBalance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WalletBalance",
                table: "Users",
                newName: "Wallet");
        }
    }
}
