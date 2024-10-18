using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGamesStore.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCategoryBehaviorChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardGames_Categories_CategoryID",
                table: "BoardGames");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardGames_Categories_CategoryID",
                table: "BoardGames",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardGames_Categories_CategoryID",
                table: "BoardGames");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardGames_Categories_CategoryID",
                table: "BoardGames",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID");
        }
    }
}
