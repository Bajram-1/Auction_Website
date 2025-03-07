using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auction_Website.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedRemainingFieldsInAuctionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_CreatedByUserId",
                table: "Auctions");

            migrationBuilder.AlterColumn<bool>(
                name: "IsClosed",
                table: "Auctions",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Auctions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ApplicationUserId",
                table: "Auctions",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_ApplicationUserId",
                table: "Auctions",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_CreatedByUserId",
                table: "Auctions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_ApplicationUserId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_CreatedByUserId",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_ApplicationUserId",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Auctions");

            migrationBuilder.AlterColumn<bool>(
                name: "IsClosed",
                table: "Auctions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_CreatedByUserId",
                table: "Auctions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
